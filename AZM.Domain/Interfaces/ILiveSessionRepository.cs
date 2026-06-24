using AZM.Domain.Entities;

namespace AZM.Domain.Interfaces
{
    public interface ILiveSessionRepository
    {
        Task<LiveSession?> GetByIdAsync(Guid id);
        Task<LiveSession?> GetActiveByEventIdAsync(Guid eventId);
        Task<List<LiveSession>> GetStaleSessionsAsync(TimeSpan idleThreshold);
        Task<List<LiveSession>> GetByEventIdAsync(Guid eventId);
        Task AddAsync(LiveSession session, CancellationToken ct = default);
        Task UpdateAsync(LiveSession session, CancellationToken ct = default);
    }
}