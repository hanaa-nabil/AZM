using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using AZM.Infrastructure.DbContext;
using AZM.Infrastructure.Identity;
using AZM.Infrastructure.Notifications;
using AZM.Infrastructure.Repositories;
using AZM.Infrastructure.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            // ── 4. HANGFIRE ──────────────────────────────────────────
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

            // ── 5. REPOSITORIES ──────────────────────────────────────
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // ── 6. NOTIFICATIONS (FCM) ────────────────────────────────
            services.Configure<FcmOptions>(configuration.GetSection(FcmOptions.SectionName));

            // Initialize Firebase ONCE here at startup, not inside FcmNotificationService's
            // constructor — that service is Scoped and gets re-constructed on every Hangfire
            // job tick, which previously meant re-reading the credential file (and throwing)
            // every single time it was missing.
            var fcmKeyPath = configuration[$"{FcmOptions.SectionName}:ServiceAccountKeyPath"];
            if (!string.IsNullOrWhiteSpace(fcmKeyPath) && File.Exists(fcmKeyPath) && FirebaseApp.DefaultInstance is null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(fcmKeyPath)
                });
            }

            services.AddScoped<INotificationService, FcmNotificationService>();

            // ── 7. SERVICES ───────────────────────────────────────────
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<ISocialAuthService, GoogleAuthService>();
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