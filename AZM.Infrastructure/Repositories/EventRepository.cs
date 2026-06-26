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

        public EventRepository(AppDbContext db) => _db = db;

        // ── Basic ─────────────────────────────────────────────────────────────────

        public async Task<Event?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _db.Events
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == id, ct);

        public async Task<Event?> GetByIdWithParticipantsAsync(Guid id, CancellationToken ct = default)
            => await _db.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                    .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(e => e.Id == id, ct);

        // ── Feed ──────────────────────────────────────────────────────────────────

        public async Task<(IEnumerable<Event> Events, int TotalCount)> GetFeedAsync(
            int page, int pageSize,
            SportType? sportType = null,
            EventStatus? status = null,
            CancellationToken ct = default)
        {
            var query = _db.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .AsQueryable();

            if (sportType.HasValue)
                query = query.Where(e => e.SportType == sportType.Value);

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);
            else
                query = query.Where(e => e.Status != EventStatus.Cancelled);

            var total = await query.CountAsync(ct);
            var events = await query
                .OrderByDescending(e => e.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (events, total);
        }

        // ── Nearby (Haversine in memory — acceptable for moderate data) ────────────

        public async Task<IEnumerable<Event>> GetNearbyAsync(
            double lat, double lng, double radiusKm, CancellationToken ct = default)
        {
            // Pull candidates within a bounding box first for DB efficiency
            double latDelta = radiusKm / 111.0;
            double lngDelta = radiusKm / (111.0 * Math.Cos(lat * Math.PI / 180));

            var candidates = await _db.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .Where(e =>
                    e.Status != EventStatus.Cancelled &&
                    e.Latitude >= lat - latDelta && e.Latitude <= lat + latDelta &&
                    e.Longitude >= lng - lngDelta && e.Longitude <= lng + lngDelta)
                .ToListAsync(ct);

            // Exact Haversine filter
            return candidates.Where(e => Haversine(lat, lng, e.Latitude, e.Longitude) <= radiusKm)
                .OrderBy(e => Haversine(lat, lng, e.Latitude, e.Longitude));
        }

        private static double Haversine(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLon = (lon2 - lon1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                  + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
                  * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        // ── Organizer / User ──────────────────────────────────────────────────────

        public async Task<IEnumerable<Event>> GetByOrganizerAsync(Guid organizerId, CancellationToken ct = default)
            => await _db.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .Where(e => e.OrganizerId == organizerId)
                .OrderByDescending(e => e.EventDate)
                .ToListAsync(ct);
       
        public async Task<IEnumerable<Event>> GetUserJoinedEventsAsync(Guid userId, CancellationToken ct = default)
            => await _db.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants)
                .Where(e => e.Participants.Any(p =>
                    p.UserId == userId &&
                    p.Status == ParticipantStatus.Joined))
                .OrderByDescending(e => e.EventDate)
                .ToListAsync(ct);

        public async Task<IEnumerable<Event>> GetStartingWithinAsync(TimeSpan window, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            var cutoff = now.Add(window);

            return await _db.Events
                .Include(e => e.Organizer)
                .Include(e => e.Participants) 
                .Where(e =>
                    e.Status != EventStatus.Cancelled &&
                    e.EventDate >= now &&
                    e.EventDate <= cutoff)
                .OrderBy(e => e.EventDate)
                .ToListAsync(ct);
        }

        // ── CRUD ──────────────────────────────────────────────────────────────────

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

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
            => await _db.Events.AnyAsync(e => e.Id == id, ct);

        // ── Participants ──────────────────────────────────────────────────────────

        public async Task<EventParticipant?> GetParticipantAsync(Guid eventId, Guid userId, CancellationToken ct = default)
            => await _db.EventParticipants
                .FirstOrDefaultAsync(p => p.EventId == eventId && p.UserId == userId, ct);

        public async Task<IEnumerable<EventParticipant>> GetParticipantsAsync(Guid eventId, CancellationToken ct = default)
            => await _db.EventParticipants
                .Include(p => p.User)
                .Where(p => p.EventId == eventId && p.Status == ParticipantStatus.Joined)
                .ToListAsync(ct);

        public async Task AddParticipantAsync(EventParticipant participant, CancellationToken ct = default)
        {
            await _db.EventParticipants.AddAsync(participant, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateParticipantAsync(EventParticipant participant, CancellationToken ct = default)
        {
            _db.EventParticipants.Update(participant);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<bool> IsParticipantAsync(Guid eventId, Guid userId, CancellationToken ct = default)
            => await _db.EventParticipants.AnyAsync(p =>
                p.EventId == eventId &&
                p.UserId == userId &&
                p.Status == ParticipantStatus.Joined, ct);

        public async Task<int> GetParticipantCountAsync(Guid eventId, CancellationToken ct = default)
            => await _db.EventParticipants.CountAsync(p =>
                p.EventId == eventId &&
                p.Status == ParticipantStatus.Joined, ct);
    }

}