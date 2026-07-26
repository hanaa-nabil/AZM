using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.User
{
    public class AchievementDto
    {
        public Guid DefinitionId { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string IconUrl { get; set; } = default!;
        public bool IsEarned { get; set; }
        public DateTime? EarnedAtUtc { get; set; } 
    }
}
