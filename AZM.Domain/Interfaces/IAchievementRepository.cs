using AZM.Domain.Entities;
using AZM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Interfaces
{
    public interface IAchievementRepository
    {
        Task<AchievementDefinition?> GetByCriteriaAsync(AchievementCriteriaType type, int threshold);
        Task<bool> AlreadyEarnedAsync(Guid userId, Guid definitionId);
        Task GrantAsync(Guid userId, Guid definitionId);
        Task<List<Achievement>> GetEarnedByUserAsync(Guid userId);
        Task<List<AchievementDefinition>> GetAllDefinitionsAsync();
    }
}
