using AZM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class LiveSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? EndedAtUtc { get; set; }

        public SessionStatus Status { get; set; } = SessionStatus.Active;

        // ----- Owner of this tracking session -----
        public string UserId { get; set; } = string.Empty;
        public User? User { get; set; }

        // Optional link to the event being tracked, if this session belongs to one
        public Guid? EventId { get; set; }
        public Event? Event { get; set; }

        // ----- GPS pings recorded during this session -----
        public ICollection<LiveLocation> Locations { get; set; } = new List<LiveLocation>();
    }
}
