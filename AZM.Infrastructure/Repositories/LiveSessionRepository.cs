using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using AZM.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Repositories
{
    public class LiveSessionRepository : ILiveSessionRepository
    {
        private readonly AppDbContext _db;

        public LiveSessionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<LiveSession?> GetByIdAsync(Guid id)
        {
            return await _db.LiveSessions
                .Include(s => s.Locations)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        // Only one active session per event at a time
        public async Task<LiveSession?> GetActiveByEventIdAsync(Guid eventId)
        {
            return await _db.LiveSessions
                .FirstOrDefaultAsync(s => s.EventId == eventId
                                       && s.Status == SessionStatus.Active);
        }

        // Used by Hangfire expire job — finds sessions idle over 6 hours
        public async Task<List<LiveSession>> GetStaleSessionsAsync(TimeSpan idleThreshold)
        {
            var cutoff = DateTime.UtcNow.Subtract(idleThreshold);

            return await _db.LiveSessions
                .Where(s => s.Status == SessionStatus.Active
                         && s.StartedAtUtc < cutoff)
                .ToListAsync();
        }

        public async Task<List<LiveSession>> GetByEventIdAsync(Guid eventId)
        {
            return await _db.LiveSessions
                .Where(s => s.EventId == eventId)
                .OrderByDescending(s => s.StartedAtUtc)
                .ToListAsync();
        }

        public async Task AddAsync(LiveSession session, CancellationToken ct = default)
        {
            await _db.LiveSessions.AddAsync(session, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(LiveSession session, CancellationToken ct = default)
        {
            _db.LiveSessions.Update(session);
            await _db.SaveChangesAsync(ct);
        }
    }
}
