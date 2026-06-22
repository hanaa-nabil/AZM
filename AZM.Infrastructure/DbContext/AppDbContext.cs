using AZM.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.DbContext
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventParticipant> EventParticipants { get; set; } = null!;
        public DbSet<EventRoute> EventRoutes { get; set; } = null!;
        public DbSet<EventRouteWaypoint> EventRouteWaypoints { get; set; } = null!;
        public DbSet<LiveLocation> LiveLocations { get; set; } = null!;
        public DbSet<LiveSession> LiveSessions { get; set; } = null!;
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<Achievement> Achievements { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ----- Event <-> EventRoute (1-to-1) -----
            builder.Entity<EventRoute>()
                .HasOne(r => r.Event)
                .WithOne(e => e.Route)
                .HasForeignKey<EventRoute>(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

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

            // ----- User <-> LiveSession (1-to-many) -----
            builder.Entity<LiveSession>()
                .HasOne(s => s.User)
                .WithMany(u => u.LiveSessions)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ----- Event <-> LiveSession (optional 1-to-many) -----
            builder.Entity<LiveSession>()
                .HasOne(s => s.Event)
                .WithMany()
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.SetNull);

            // ----- LiveSession <-> LiveLocation (1-to-many) -----
            builder.Entity<LiveLocation>()
                .HasOne(l => l.LiveSession)
                .WithMany(s => s.Locations)
                .HasForeignKey(l => l.LiveSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ----- EventRoute <-> EventRouteWaypoint (1-to-many) -----
            builder.Entity<EventRouteWaypoint>()
                .HasOne(w => w.EventRoute)
                .WithMany(r => r.Waypoints)
                .HasForeignKey(w => w.EventRouteId)
                .OnDelete(DeleteBehavior.Cascade);

            // ----- User <-> Achievement (1-to-many) -----
            builder.Entity<Achievement>()
                .HasOne(a => a.User)
                .WithMany(u => u.Achievements)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ----- Event <-> User (creator, 1-to-many, no inverse navigation) -----
            builder.Entity<Event>()
                .HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
