using AZM.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

namespace AZM
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ── Infrastructure (DB, Identity, JWT, Email, Repositories) ──
            builder.Services.AddInfrastructure(builder.Configuration);

            // ── MediatR (scans Application layer for all command handlers) ──
            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(
                    typeof(AZM.Application.Auth.Handlers.RegisterCommandHandler).Assembly));

            // ── JWT Authentication ──
            var jwtSection = builder.Configuration.GetSection("JwtSettings");
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!))
                };
            });

            builder.Services.AddAuthorization();

            // ── Rate Limiting ──
            builder.Services.AddRateLimiter(options =>
            {
                options.AddPolicy("registration", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1)
                        }));

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            // ── CORS ──
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DefaultCorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin()   // tighten this when you go to production
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            // ── Controllers + Swagger ──
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new() { Title = "AZM API", Version = "v1" });
                options.AddSecurityDefinition("Bearer", new()
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Enter: Bearer {your token}"
                });
                options.AddSecurityRequirement(new()
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var app = builder.Build();
            app.UseHangfireDashboard("/hangfire");

            // Schedule your recurring jobs here
            RecurringJob.AddOrUpdate<EventReminderJob>(
                "event-reminders",
                job => job.RunAsync(),
                Cron.Minutely);   // checks every minute, sends notification if event is in 1hr

            RecurringJob.AddOrUpdate<ExpireStaleSessionsJob>(
                "expire-stale-sessions",
                job => job.RunAsync(),
                Cron.Hourly);     // checks every hour, ends sessions idle over 6hrs

            app.MapHub<MapHub>("/hubs/map");  // SignalR hub endpoint

            // Seed roles
            using (var scope = app.Services.CreateScope())
            {
                await InfrastructureServiceExtensions.SeedRolesAsync(scope.ServiceProvider);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("DefaultCorsPolicy");
            app.UseRateLimiter();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}