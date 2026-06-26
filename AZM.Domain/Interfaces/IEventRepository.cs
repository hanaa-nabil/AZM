using AZM.Domain.Entities;
using AZM.Domain.Enums;
using System.Reflection.PortableExecutable;

namespace AZM.Domain.Interfaces
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Event?> GetByIdWithParticipantsAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Event>> GetStartingWithinAsync(TimeSpan window, CancellationToken ct = default);
        Task<(IEnumerable<Event> Events, int TotalCount)> GetFeedAsync(
            int page,
            int pageSize,
            SportType? sportType = null,
            EventStatus? status = null,
            CancellationToken ct = default);
        Task<IEnumerable<Event>> GetNearbyAsync(double lat, double lng, double radiusKm, CancellationToken ct = default);
        Task<IEnumerable<Event>> GetByOrganizerAsync(Guid organizerId, CancellationToken ct = default);
        Task<IEnumerable<Event>> GetUserJoinedEventsAsync(Guid userId, CancellationToken ct = default);
        Task AddAsync(Event ev, CancellationToken ct = default);
        Task UpdateAsync(Event ev, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);

        // Participant specific
        Task<EventParticipant?> GetParticipantAsync(Guid eventId, Guid userId, CancellationToken ct = default);
        Task<IEnumerable<EventParticipant>> GetParticipantsAsync(Guid eventId, CancellationToken ct = default);
        Task AddParticipantAsync(EventParticipant participant, CancellationToken ct = default);
        Task UpdateParticipantAsync(EventParticipant participant, CancellationToken ct = default);
        Task<bool> IsParticipantAsync(Guid eventId, Guid userId, CancellationToken ct = default);
        Task<int> GetParticipantCountAsync(Guid eventId, CancellationToken ct = default);
    }
}