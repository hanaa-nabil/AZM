using AZM.Domain.Entities;
using AZM.Domain.Enums;
using System.Reflection.PortableExecutable;

namespace AZM.Domain.Interfaces
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(Guid id);
        Task<List<Event>> GetNearbyAsync(double lat, double lng, int radiusMeters, SportType? sportType = null);
        Task<List<Event>> GetByCreatorAsync(string userId);
        Task<List<Event>> GetStartingWithinAsync(TimeSpan window);
        Task AddAsync(Event ev, CancellationToken ct = default);
        Task UpdateAsync(Event ev, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);
        
    }
}