using AZM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class EventParticipant
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public ParticipantStatus Status { get; set; } = ParticipantStatus.Invited;

        public DateTime JoinedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? CheckedInAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }

        // ----- Foreign keys -----
        public Guid EventId { get; set; }
        public Event? Event { get; set; }

        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }
    }
}
