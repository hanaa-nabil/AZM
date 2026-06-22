using AZM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DifficultyLevel Difficulty { get; set; }
        public EventStatus Status { get; set; } = EventStatus.Scheduled;

        public DateTime StartAtUtc { get; set; }
        public DateTime? EndAtUtc { get; set; }

        public int? MaxParticipants { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string CreatedByUserId { get; set; } = string.Empty;
        public User? CreatedByUser { get; set; }

        public EventRoute? Route { get; set; }

        public ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();
    }
}
