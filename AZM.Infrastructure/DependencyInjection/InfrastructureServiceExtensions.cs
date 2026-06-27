using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using AZM.Infrastructure.DbContext;
using AZM.Infrastructure.Identity;
using AZM.Infrastructure.Notifications;
using AZM.Infrastructure.Repositories;
using AZM.Infrastructure.Services;
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
            // 1. DATABASE
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // 2. IDENTITY
            services.AddIdentity<User, IdentityRole<Guid>>(options =>
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

            // 3. JWT
            services.Configure<JwtSettings>(
                configuration.GetSection("JwtSettings"));

            // 4. HANGFIRE
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

            services.AddHangfireServer();

            // 5. REPOSITORIES
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // 6. NOTIFICATIONS
            services.Configure<FcmOptions>(
                configuration.GetSection(FcmOptions.SectionName));
            services.AddScoped<INotificationService, FcmNotificationService>();

            // 7. SERVICES
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOtpService, OtpService>();
            services.AddScoped<ISocialAuthService, GoogleAuthService>();
            services.Configure<CloudinarySettings>(
                configuration.GetSection(CloudinarySettings.SectionName));
            services.AddScoped<IPhotoService, CloudinaryPhotoService>();
            services.AddScoped<IPasswordHasher<OtpCode>, PasswordHasher<OtpCode>>();
            services.AddHttpClient<IFirebaseAuthService, FirebaseAuthService>();

            return services;
        }

        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            string[] roles = { "ATHLETE", "ORGANIZER", "ADMIN" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}