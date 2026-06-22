using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class UserProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }

        public int EventsJoinedCount { get; set; }
        public int EventsCompletedCount { get; set; }
        public double TotalDistanceMeters { get; set; }

        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}
