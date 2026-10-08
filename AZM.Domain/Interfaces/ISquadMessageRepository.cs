using AZM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Interfaces
{
    public interface ISquadMessageRepository
    {
        Task AddAsync(SquadMessage message, CancellationToken ct = default);
        Task<SquadMessage?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task UpdateAsync(SquadMessage message, CancellationToken ct = default);
        Task<List<SquadMessage>> GetPageAsync(Guid squadId, int page, int pageSize, CancellationToken ct = default);
    }
}
