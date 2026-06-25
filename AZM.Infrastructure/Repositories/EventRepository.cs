using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using AZM.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace AZM.Infrastructure.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _db;

        public EventRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            return await _db.Events
                .Include(e => e.Participants)
                    .ThenInclude(p => p.User)
                .Include(e => e.CreatedByUser)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Event>> GetNearbyAsync(
            double lat, double lng, int radiusMeters, SportType? sportType = null)
        {
            const double R = 6371000;

            return await _db.Events
                .Include(e => e.Participants)
                .Include(e => e.CreatedByUser)
                .Where(e => e.Status != EventStatus.Cancelled)
                .Where(e => sportType == null || e.SportType == sportType)
                .Where(e =>
                    R * 2 * Math.Asin(Math.Sqrt(
                        Math.Pow(Math.Sin((e.MeetingLat - lat) * Math.PI / 180 / 2), 2) +
                        Math.Cos(lat * Math.PI / 180) *
                        Math.Cos(e.MeetingLat * Math.PI / 180) *
                        Math.Pow(Math.Sin((e.MeetingLng - lng) * Math.PI / 180 / 2), 2)
                    )) <= radiusMeters
                )
                .OrderBy(e => e.StartAtUtc)
                .ToListAsync();
        }

        public async Task<List<Event>> GetByCreatorAsync(string userId)
        {
            return await _db.Events
                .Where(e => e.CreatedByUserId == userId)
                .OrderByDescending(e => e.StartAtUtc)
                .ToListAsync();
        }

        public async Task<List<Event>> GetStartingWithinAsync(TimeSpan window)
        {
            var from = DateTime.UtcNow;
            var to = DateTime.UtcNow.Add(window);
            return await _db.Events
                .Include(e => e.Participants)
                .Where(e => e.Status == EventStatus.Scheduled
                         && e.StartAtUtc >= from
                         && e.StartAtUtc <= to)
                .ToListAsync();
        }

        public async Task AddAsync(Event ev, CancellationToken ct = default)
        {
            await _db.Events.AddAsync(ev, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Event ev, CancellationToken ct = default)
        {
            _db.Events.Update(ev);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var ev = await _db.Events.FindAsync(id);
            if (ev is null) return;
            _db.Events.Remove(ev);
            await _db.SaveChangesAsync(ct);
        }
    }
}