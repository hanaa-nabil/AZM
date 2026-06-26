using AZM.Domain.Enums;

namespace AZM.Domain.Entities
{
    public class EventParticipant
    {
        public Guid Id { get; private set; }
        public Guid EventId { get; private set; }
        public Event Event { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public ParticipantStatus Status { get; private set; }
        public DateTime JoinedAt { get; private set; }
        public DateTime? LeftAt { get; private set; }

        private EventParticipant() { }

        public static EventParticipant Create(Guid eventId, Guid userId)
        {
            return new EventParticipant
            {
                Id = Guid.NewGuid(),
                EventId = eventId,
                UserId = userId,
                Status = ParticipantStatus.Joined,
                JoinedAt = DateTime.UtcNow
            };
        }

        public void Leave()
        {
            Status = ParticipantStatus.Left;
            LeftAt = DateTime.UtcNow;
        }

        public void Rejoin()
        {
            Status = ParticipantStatus.Joined;
            LeftAt = null;
        }
    }
}
