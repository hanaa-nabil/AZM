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
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Title).HasMaxLength(200).IsRequired();
            builder.Property(n => n.Body).HasMaxLength(1000).IsRequired();
            builder.Property(n => n.Type).HasConversion<string>();

            builder.HasOne(n => n.Recipient)
                .WithMany()
                .HasForeignKey(n => n.RecipientId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(n => n.RelatedEvent)
                .WithMany()
                .HasForeignKey(n => n.RelatedEventId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasIndex(n => new { n.RecipientId, n.IsRead });
        }
    }
}
