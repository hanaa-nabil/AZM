using AZM.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(e => e.LocationName)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.SportType)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(e => e.DifficultyLevel)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(e => e.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(e => e.Latitude).IsRequired();
            builder.Property(e => e.Longitude).IsRequired();

            builder.Property(e => e.CoverImageUrl).HasMaxLength(1000);
            builder.Property(e => e.RouteImageUrl).HasMaxLength(1000);

            // Organizer FK
            builder.HasOne(e => e.Organizer)
                .WithMany()
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Participants nav
            builder.HasMany(e => e.Participants)
                .WithOne(p => p.Event)
                .HasForeignKey(p => p.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for feed queries
            builder.HasIndex(e => e.Status);
            builder.HasIndex(e => e.SportType);
            builder.HasIndex(e => e.EventDate);
            builder.HasIndex(e => e.OrganizerId);
            builder.HasIndex(e => new { e.Latitude, e.Longitude }); // For nearby queries
        }
    }

    public class EventParticipantConfiguration : IEntityTypeConfiguration<EventParticipant>
    {
        public void Configure(EntityTypeBuilder<EventParticipant> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint: one record per user per event
            builder.HasIndex(p => new { p.EventId, p.UserId }).IsUnique();
            builder.HasIndex(p => p.UserId);
        }
    }
}
