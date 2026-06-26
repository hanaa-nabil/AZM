using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using AZM.Infrastructure.DbContext;
using AZM.Infrastructure.Identity;
<<<<<<< HEAD
using AZM.Infrastructure.Repositories;
using AZM.Infrastructure.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
=======
using AZM.Infrastructure.Notifications;
using AZM.Infrastructure.Repositories;
using AZM.Infrastructure.Services;
>>>>>>> DB Back to local, Auth working technically
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

<<<<<<< HEAD
            // Identity
=======
            // 2. IDENTITY
>>>>>>> DB Back to local, Auth working technically
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

<<<<<<< HEAD
            // JWT settings
            services.Configure<JwtSettings>(options =>
                configuration.GetSection("JwtSettings").Bind(options));

            // Services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IOtpService, OtpService>();
=======
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
>>>>>>> DB Back to local, Auth working technically
            services.AddScoped<IPasswordHasher<OtpCode>, PasswordHasher<OtpCode>>();

            return services;
        }

        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
<<<<<<< HEAD
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

=======
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
>>>>>>> DB Back to local, Auth working technically
            string[] roles = { "ATHLETE", "ORGANIZER", "ADMIN" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }
    }
}