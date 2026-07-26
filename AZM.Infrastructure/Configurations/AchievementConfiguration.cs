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
    public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
    {
        public void Configure(EntityTypeBuilder<Achievement> builder)
        {
            builder.HasKey(a => a.Id);

            builder.HasOne(a => a.AchievementDefinition)
                .WithMany()
                .HasForeignKey(a => a.AchievementDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(a => new { a.UserId, a.AchievementDefinitionId }).IsUnique();
        }
    }
}
