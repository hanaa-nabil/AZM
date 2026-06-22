using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class Achievement
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        //public string? IconUrl { get; set; }

        public DateTime EarnedAtUtc { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty; // string because IdentityUser.Id is a string
        public User? User { get; set; }
    }
}
