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
    public class AchievementDefinitionConfiguration : IEntityTypeConfiguration<AchievementDefinition>
    {
        public void Configure(EntityTypeBuilder<AchievementDefinition> builder)
        {
            builder.HasKey(d => d.Id);
            builder.Property(d => d.Code).IsRequired().HasMaxLength(64);
            builder.Property(d => d.Name).IsRequired().HasMaxLength(128);
            builder.HasIndex(d => d.Code).IsUnique();
        }
    }
}
