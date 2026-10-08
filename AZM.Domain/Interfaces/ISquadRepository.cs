using AZM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Interfaces
{
    public interface ISquadRepository
    {
        Task<Squad?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<Squad?> GetByIdWithMembersAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(Squad squad, CancellationToken ct = default);
        Task UpdateAsync(Squad squad, CancellationToken ct = default);
        Task DeleteAsync(Guid id, CancellationToken ct = default);

        Task<SquadMember?> GetMemberAsync(Guid squadId, Guid userId, CancellationToken ct = default);
        Task AddMemberAsync(SquadMember member, CancellationToken ct = default);
        Task UpdateMemberAsync(SquadMember member, CancellationToken ct = default);
        Task<List<SquadMember>> GetRosterAsync(Guid squadId, CancellationToken ct = default);
        Task<List<SquadMember>> GetPendingRequestsAsync(Guid squadId, CancellationToken ct = default);

        Task<List<Squad>> GetMySquadsAsync(Guid userId, CancellationToken ct = default);
        Task<List<Squad>> GetNearbyAsync(double lat, double lng, double radiusKm, CancellationToken ct = default);
        Task<List<Squad>> SearchAsync(string term, CancellationToken ct = default);
        Task<List<Guid>> GetCoMemberIdsAsync(Guid userId, CancellationToken ct = default);
    }
}
