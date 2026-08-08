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
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            await _context.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetValidTokenAsync(string token)
        {
            return await _context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == token
                    && !t.IsRevoked
                    && t.ExpiresAtUtc > DateTime.UtcNow);
        }

        public async Task RevokeAsync(string token)
        {
            await _context.RefreshTokens
                .Where(t => t.Token == token)
                .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.IsRevoked, true));
        }

        public async Task RevokeAllForUserAsync(Guid userId)
        {
            await _context.RefreshTokens
                .Where(t => t.UserId == userId && !t.IsRevoked)
                .ExecuteUpdateAsync(setters => setters.SetProperty(t => t.IsRevoked, true));
        }
    }
}
