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

        // Meeting point — where runners gather before the run
        public double MeetingLat { get; set; }
        public double MeetingLng { get; set; }
        public string MeetingAddress { get; set; } = string.Empty;

        // Estimated run duration in minutes (from Google Distance Matrix)
        public int EstimatedMinutes { get; set; }

        // Creator
        public string CreatedByUserId { get; set; } = string.Empty;
        public User? CreatedByUser { get; set; }

        // Navigation properties
        public EventRoute? Route { get; set; }
        public ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();
        public ICollection<LiveSession> LiveSessions { get; set; } = new List<LiveSession>();

        // ── Domain methods ──────────────────────────────────────────

        public void Publish()
        {
            if (Status != EventStatus.Draft)
                throw new InvalidOperationException("Only draft events can be published");

            Status = EventStatus.Published;
        }

        public void Start()
        {
            if (Status != EventStatus.Published)
                throw new InvalidOperationException("Only published events can be started");

            Status = EventStatus.Active;
        }

        public void Complete()
        {
            if (Status != EventStatus.Active)
                throw new InvalidOperationException("Only active events can be completed");

            Status = EventStatus.Completed;
            EndAtUtc = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if (Status == EventStatus.Completed)
                throw new InvalidOperationException("Cannot cancel a completed event");

            Status = EventStatus.Cancelled;
        }

        public bool IsFull =>
            MaxParticipants.HasValue &&
            Participants.Count(p => p.Status == ParticipantStatus.Joined) >= MaxParticipants.Value;
    }
}
