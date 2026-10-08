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
    public class SquadRepository : ISquadRepository
    {
        private readonly AppDbContext _db;
        public SquadRepository(AppDbContext db) => _db = db;

        public async Task<Squad?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await _db.Squads.Include(s => s.Founder).FirstOrDefaultAsync(s => s.Id == id, ct);

        public async Task<Squad?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default)
            => await _db.Squads
                .Include(s => s.Founder)
                .Include(s => s.Members).ThenInclude(m => m.User)
                .FirstOrDefaultAsync(s => s.Id == id, ct);

        public async Task AddAsync(Squad squad, CancellationToken ct = default)
        {
            await _db.Squads.AddAsync(squad, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Squad squad, CancellationToken ct = default)
        {
            _db.Squads.Update(squad);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            await _db.Squads.Where(s => s.Id == id).ExecuteDeleteAsync(ct);
        }

        public async Task<SquadMember?> GetMemberAsync(Guid squadId, Guid userId, CancellationToken ct = default)
            => await _db.SquadMembers
                .FirstOrDefaultAsync(m => m.SquadId == squadId && m.UserId == userId, ct);

        public async Task AddMemberAsync(SquadMember member, CancellationToken ct = default)
        {
            await _db.SquadMembers.AddAsync(member, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateMemberAsync(SquadMember member, CancellationToken ct = default)
        {
            _db.SquadMembers.Update(member);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<List<SquadMember>> GetRosterAsync(Guid squadId, CancellationToken ct = default)
            => await _db.SquadMembers
                .Include(m => m.User)
                .Where(m => m.SquadId == squadId && m.Status == SquadMemberStatus.Approved)
                .ToListAsync(ct);

        public async Task<List<SquadMember>> GetPendingRequestsAsync(Guid squadId, CancellationToken ct = default)
            => await _db.SquadMembers
                .Include(m => m.User)
                .Where(m => m.SquadId == squadId && m.Status == SquadMemberStatus.Pending)
                .ToListAsync(ct);

      
        public async Task<List<Squad>> GetMySquadsAsync(Guid userId, CancellationToken ct = default)
              => await _db.Squads
                 .Include(s => s.Founder)
                 .Include(s => s.Members)
                 .Where(s => s.Members.Any(m => m.UserId == userId && m.Status == SquadMemberStatus.Approved))
                 .ToListAsync(ct);
        public async Task<List<Squad>> GetNearbyAsync(double lat, double lng, double radiusKm, CancellationToken ct = default)
        {
            double latDelta = radiusKm / 111.0;
            double lngDelta = radiusKm / (111.0 * Math.Cos(lat * Math.PI / 180));

            return await _db.Squads
                .Include(s => s.Founder)
                .Include(s => s.Members)
                .Where(s => s.Privacy == SquadPrivacy.Public &&
                            s.Latitude != null && s.Longitude != null &&
                            s.Latitude >= lat - latDelta && s.Latitude <= lat + latDelta &&
                            s.Longitude >= lng - lngDelta && s.Longitude <= lng + lngDelta)
                .ToListAsync(ct);
        }

        public async Task<List<Squad>> SearchAsync(string term, CancellationToken ct = default)
            => await _db.Squads
                .Include(s => s.Founder)
                .Include(s => s.Members)
                .Where(s => EF.Functions.Like(s.Name, $"%{term}%"))
                .ToListAsync(ct);


        public async Task<List<Guid>> GetCoMemberIdsAsync(Guid userId, CancellationToken ct = default)
        {
            var mySquadIds = _db.SquadMembers
                .Where(m => m.UserId == userId && m.Status == SquadMemberStatus.Approved)
                .Select(m => m.SquadId);

            return await _db.SquadMembers
              .Where(m => mySquadIds.Contains(m.SquadId) && m.UserId != userId
                          && m.Status == SquadMemberStatus.Approved)
              .Select(m => m.UserId).Distinct().ToListAsync(ct);
        }


      
    }
}
