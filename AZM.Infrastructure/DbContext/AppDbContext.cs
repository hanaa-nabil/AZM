using AZM.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AZM.Infrastructure.DbContext
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventParticipant> EventParticipants { get; set; } = null!;
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<Achievement> Achievements { get; set; } = null!;
        public DbSet<OtpCode> OtpCodes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            // ----- Event <-> EventParticipant (1-to-many) -----
            builder.Entity<EventParticipant>()
                .HasOne(p => p.Event)
                .WithMany(e => e.Participants)
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // ----- User <-> EventParticipant (1-to-many) -----
            builder.Entity<EventParticipant>()
                .HasOne(p => p.User)
                .WithMany(u => u.EventParticipants)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ----- User <-> UserProfile (1-to-1) -----
            builder.Entity<UserProfile>()
                .HasOne(p => p.User)
                .WithOne(u => u.Profile)
                .HasForeignKey<UserProfile>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // ----- User <-> Achievement (1-to-many) -----
            builder.Entity<Achievement>()
                .HasOne(a => a.User)
                .WithMany(u => u.Achievements)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ----- Event <-> User (creator, no cascade to avoid cycles) -----
            builder.Entity<Event>()
                .HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- OtpCode — index on Email for fast lookup -----  ← NEW
            builder.Entity<OtpCode>()
                .HasIndex(o => o.Email);
        }
    }
}