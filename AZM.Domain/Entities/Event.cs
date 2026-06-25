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
        public EventStatus Status { get; set; } = EventStatus.Draft;

        public DateTime StartAtUtc { get; set; }
        public DateTime? EndAtUtc { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public int? MaxParticipants { get; set; }
        public SportType SportType { get; set; }
        public double MeetingLat { get; set; }
        public double MeetingLng { get; set; }
        public string MeetingAddress { get; set; } = string.Empty;  

        public string CreatedByUserId { get; set; } = string.Empty;
        public User? CreatedByUser { get; set; }

        public ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();

        public bool IsFull =>
            MaxParticipants.HasValue &&
            Participants.Count(p => p.Status == ParticipantStatus.Joined) >= MaxParticipants.Value;

        public void Publish()
        {
            if (Status != EventStatus.Draft)
                throw new InvalidOperationException("Only draft events can be published");
            Status = EventStatus.Scheduled;
        }

        public void Cancel()
        {
            if (Status == EventStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed event");
            Status = EventStatus.Cancelled;
        }

    }
}
