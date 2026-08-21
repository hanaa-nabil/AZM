using AZM.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AZM.Infrastructure.DbContext
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventParticipant> EventParticipants { get; set; } = null!;

        public DbSet<EventRoute> EventRoutes { get; set; } = null!;
        //public DbSet<EventRouteWaypoint> EventRouteWaypoints { get; set; } = null!;

        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<OtpCode> OtpCodes { get; set; } = null!;   
        public DbSet<UserSport> UserSports { get; set; } = null!;
        public DbSet<AchievementDefinition> AchievementDefinitions => Set<AchievementDefinition>();
        public DbSet<Achievement> Achievements => Set<Achievement>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<UserDailyActivity> UserDailyActivities => Set<UserDailyActivity>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Follow> Follows => Set<Follow>();
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<EventParticipant>()
                .HasOne(p => p.Event)
                .WithMany(e => e.Participants)
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // User <-> EventParticipant (1-to-many)
            builder.Entity<EventParticipant>()
                .HasOne(p => p.User)
                .WithMany(u => u.EventParticipants)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User <-> UserProfile (1-to-1)
            builder.Entity<UserProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            // Achievement -> AchievementDefinition (many-to-one)
            builder.Entity<Achievement>()
    .HasOne<User>()
    .WithMany(u => u.Achievements) // remove ".WithMany(u => u.Achievements)" args if User has no such collection — use .HasForeignKey below only
    .HasForeignKey(a => a.UserId)
    .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Achievement>()
                .HasOne(a => a.AchievementDefinition)
                .WithMany()
                .HasForeignKey(a => a.AchievementDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Achievement>()
                .HasIndex(a => new { a.UserId, a.AchievementDefinitionId })
                .IsUnique();

            builder.Entity<AchievementDefinition>()
                .HasIndex(d => d.Code)
                .IsUnique();

            builder.Entity<Notification>()
                .HasIndex(n => new { n.RecipientId, n.Type, n.CreatedAt });


            // Event <-> User creator (no cascade)
            builder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany()
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Event → Route (one-to-one, optional)
            builder.Entity<Event>()
                .HasOne(e => e.Route)
                .WithOne(r => r.Event)
                .HasForeignKey<EventRoute>(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // IsPublic default
            builder.Entity<Event>()
                .Property(e => e.IsPublic)
                .HasDefaultValue(true);



            // User <-> UserSport (1-to-many)
            builder.Entity<UserSport>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sports)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserSport>()
                .HasIndex(s => new { s.UserId, s.Sport })
                .IsUnique();

            // OtpCode index
            builder.Entity<OtpCode>()
                .HasIndex(o => o.Email);

            builder.Entity<UserDailyActivity>()
               .HasIndex(a => new { a.UserId, a.Date })
               .IsUnique();
            builder.Entity<RefreshToken>()
               .HasIndex(r => r.Token)
               .IsUnique();


            builder.Entity<Follow>()
                .HasIndex(f => new { f.FollowerId, f.FollowingId })
                .IsUnique();
        }
}
}