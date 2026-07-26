using AZM.Domain.DomainEvents;
using MediatR;

namespace AZM.Domain.Entities
{
    public class UserProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Bio { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Location { get; set; }
        public int EventsJoinedCount { get; set; }
        public int EventsCompletedCount { get; set; }
        public double TotalDistanceMeters { get; set; }
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
        public int CurrentStreak { get; private set; }
        public int LongestStreak { get; private set; }
        public DateOnly? LastActiveDate { get; private set; }
        public int StreakFreezesAvailable { get; private set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }

        private readonly List<INotification> _domainEvents = new();

        /// <summary>
        /// Not mapped by EF — read by whatever dispatches domain events on SaveChanges
        /// (e.g. a SaveChangesInterceptor or your unit-of-work). Call ClearDomainEvents()
        /// after dispatching.
        /// </summary>
        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

        private void AddDomainEvent(INotification domainEvent) => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents() => _domainEvents.Clear();

        /// <summary>
        /// Call this whenever the user performs a "counts as active" action
        /// (e.g. joining/completing an event). Idempotent per day.
        /// </summary>
        public void RegisterActivity(DateOnly today)
        {
            if (LastActiveDate == today)
                return; // already counted today, no-op

            if (LastActiveDate == today.AddDays(-1))
            {
                CurrentStreak++; // consecutive day
            }
            else if (LastActiveDate == today.AddDays(-2) && StreakFreezesAvailable > 0)
            {
                StreakFreezesAvailable--;
                CurrentStreak++; // freeze covered the missed day
                AddDomainEvent(new StreakFreezeUsedEvent(UserId, CurrentStreak, StreakFreezesAvailable));
            }
            else
            {
                if (CurrentStreak > 1)
                    AddDomainEvent(new StreakBrokenEvent(UserId, CurrentStreak));

                CurrentStreak = 1; // reset (today counts as day 1)
            }

            LastActiveDate = today;

            if (CurrentStreak > LongestStreak)
            {
                LongestStreak = CurrentStreak;
                AddDomainEvent(new NewBestStreakEvent(UserId, LongestStreak));
            }

            if (CurrentStreak is 3 or 7 or 14 or 30 or 100)
                AddDomainEvent(new StreakMilestoneReachedEvent(UserId, CurrentStreak));
        }

        /// <summary>
        /// Grants an extra streak freeze (e.g. reward, purchase, admin grant).
        /// </summary>
        public void GrantStreakFreeze(int count = 1)
        {
            StreakFreezesAvailable += count;
        }
    }
}