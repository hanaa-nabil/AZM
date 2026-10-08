using AZM.Domain.Entities;
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
    public class SquadMessageRepository : ISquadMessageRepository
    {
        private readonly AppDbContext _db;
        public SquadMessageRepository(AppDbContext db) => _db = db;

        public async Task AddAsync(SquadMessage message, CancellationToken ct = default)
        {
            await _db.SquadMessages.AddAsync(message, ct);
            await _db.SaveChangesAsync(ct);
        }

        public Task<SquadMessage?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _db.SquadMessages.Include(m => m.Sender).FirstOrDefaultAsync(m => m.Id == id, ct);

        public async Task UpdateAsync(SquadMessage message, CancellationToken ct = default)
        {
            _db.SquadMessages.Update(message);
            await _db.SaveChangesAsync(ct);
        }

        public Task<List<SquadMessage>> GetPageAsync(Guid squadId, int page, int pageSize, CancellationToken ct = default)
            => _db.SquadMessages.Include(m => m.Sender)
                .Where(m => m.SquadId == squadId)
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize).Take(pageSize)
                .ToListAsync(ct);
    }
}
