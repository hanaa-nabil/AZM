using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using AZM.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;

namespace AZM.Infrastructure.Repositories
{
    public class AchievementRepository : IAchievementRepository
    {
        private readonly AppDbContext _context;

        public AchievementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AchievementDefinition?> GetByCriteriaAsync(AchievementCriteriaType type, int threshold)
        {
            return await _context.AchievementDefinitions
                .FirstOrDefaultAsync(d => d.CriteriaType == type && d.Threshold == threshold && d.IsActive);
        }

        public async Task<bool> AlreadyEarnedAsync(Guid userId, Guid definitionId)
        {
            return await _context.Achievements
                .AnyAsync(a => a.UserId == userId && a.AchievementDefinitionId == definitionId);
        }


        public async Task<List<Achievement>> GetEarnedByUserAsync(Guid userId)
        {
            return await _context.Achievements
                .Where(a => a.UserId == userId)
                .Include(a => a.AchievementDefinition)
                .ToListAsync();
        }

        public async Task<List<AchievementDefinition>> GetAllDefinitionsAsync()
        {
            return await _context.AchievementDefinitions
                .Where(d => d.IsActive)
                .OrderBy(d => d.Threshold)
                .ToListAsync();
        }
        public async Task GrantAsync(Guid userId, Guid definitionId)
        {
            var def = await _context.AchievementDefinitions.FindAsync(definitionId)
                ?? throw new KeyNotFoundException("Achievement definition not found.");

            _context.Achievements.Add(new Achievement
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                AchievementDefinitionId = definitionId,
                Title = def.Name,
                Description = def.Description,
                EarnedAtUtc = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
    }
}
