using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using AZM.Infrastructure.Caching;
using AZM.Infrastructure.DbContext;
using AZM.Infrastructure.Identity;
using AZM.Infrastructure.Maps;
using AZM.Infrastructure.Notifications;
using AZM.Infrastructure.RealTime;
using AZM.Infrastructure.Repositories;
using AZM.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace AZM.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ── 1. DATABASE ──────────────────────────────────────────
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // ── 2. IDENTITY ──────────────────────────────────────────
            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // ── 3. JWT ───────────────────────────────────────────────
            services.Configure<JwtSettings>(
                configuration.GetSection("JwtSettings"));

            // ── 4. REDIS ─────────────────────────────────────────────
            // Redis is an in-memory store — we use it to hold live GPS
            // positions. It is MUCH faster than hitting the database
            // every 3 seconds per runner.
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(
                    configuration.GetConnectionString("Redis")!));

            services.AddScoped<ILocationCacheService, RedisLocationCacheService>();

            // ── 5. SIGNALR ───────────────────────────────────────────
            // SignalR keeps a persistent WebSocket connection open
            // between the server and every runner's phone.
            // When one runner moves, the server pushes that position
            // to everyone else in the same event group instantly.
            services.AddSignalR();
            services.AddScoped<LocationBroadcaster>();

            // ── 6. HANGFIRE ──────────────────────────────────────────
            // Hangfire runs scheduled background jobs.
            // We use SQL Server as the job storage (same DB you already have).
            // It creates its own tables automatically on first run.
            services.AddHangfire(h => h
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
                    configuration.GetConnectionString("DefaultConnection"),
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }));

            // This is the background worker process that picks up and runs jobs
            services.AddHangfireServer();

            // ── 7. GOOGLE MAPS ───────────────────────────────────────
            services.Configure<GoogleMapsOptions>(
                configuration.GetSection(GoogleMapsOptions.SectionName));

            services.AddHttpClient<IGoogleMapsService, GoogleMapsService>();

            // ── 8. REPOSITORIES ──────────────────────────────────────
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILiveSessionRepository, LiveSessionRepository>();

            services.Configure<FcmOptions>(configuration.GetSection(FcmOptions.SectionName));
            services.AddScoped<INotificationService, FcmNotificationService>();
            // ── 9. SERVICES ──────────────────────────────────────────
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<ISocialAuthService, GoogleAuthService>();
            services.AddScoped<INotificationService, FcmNotificationService>();
            services.AddScoped<IPasswordHasher<OtpCode>, PasswordHasher<OtpCode>>();

            return services;
        }

        // ── SEED ROLES ───────────────────────────────────────────────
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = { "ATHLETE", "ORGANIZER", "ADMIN" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}