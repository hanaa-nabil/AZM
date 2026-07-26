using AZM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class AchievementDefinition
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = default!;        // "STREAK_3", "STREAK_7", "TOP_SPEED"
        public string Name { get; set; } = default!;         // "Dawn Breaker"
        public string Description { get; set; } = default!;  // "Stay active 3 days in a row"
        public string IconUrl { get; set; } = default!;
        public AchievementCriteriaType CriteriaType { get; set; }
        public int Threshold { get; set; }                   // e.g. 3 (days), 10 (events), 1000 (meters)
        public bool IsActive { get; set; } = true;
    }
}
