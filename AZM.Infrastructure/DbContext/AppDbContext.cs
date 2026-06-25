using AZM.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace AZM.Infrastructure.DbContext
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventParticipant> EventParticipants { get; set; } = null!;
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<Achievement> Achievements { get; set; } = null!;
        public DbSet<OtpCode> OtpCodes { get; set; } = null!;   // ← NEW

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
                .HasOne(e => e.Organizer)
                .WithMany()
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ----- User <-> UserSport (1-to-many) -----
            builder.Entity<UserSport>()
                .HasOne(s => s.User)
                .WithMany(u => u.Sports)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserSport>()
                .HasIndex(s => new { s.UserId, s.Sport })
                .IsUnique(); // prevent duplicate sport entries per user

            // ----- OtpCode — index on Email for fast lookup -----  ← NEW
            builder.Entity<OtpCode>()
                .HasIndex(o => o.Email);
        }
    }
}