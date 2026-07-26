using AZM.Application.DTOs.User;
using AZM.Application.Users.Queries;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Handlers
{
    public class GetMyAchievementsHandler : IRequestHandler<GetMyAchievementsQuery, List<AchievementDto>>
    {
        private readonly IAchievementRepository _achievementRepository;

        public GetMyAchievementsHandler(IAchievementRepository achievementRepository)
        {
            _achievementRepository = achievementRepository;
        }

        public async Task<List<AchievementDto>> Handle(GetMyAchievementsQuery request, CancellationToken cancellationToken)
        {
            var allDefinitions = await _achievementRepository.GetAllDefinitionsAsync();
            var earned = await _achievementRepository.GetEarnedByUserAsync(request.UserId);
            var earnedLookup = earned.ToDictionary(a => a.AchievementDefinitionId, a => a.EarnedAtUtc);

            return allDefinitions.Select(def => new AchievementDto
            {
                DefinitionId = def.Id,
                Code = def.Code,
                Name = earnedLookup.ContainsKey(def.Id) ? def.Name : "???",
                Description = earnedLookup.ContainsKey(def.Id) ? def.Description : "Keep going to unlock this badge",
                IconUrl = earnedLookup.ContainsKey(def.Id) ? def.IconUrl : string.Empty,
                IsEarned = earnedLookup.ContainsKey(def.Id),
                EarnedAtUtc = earnedLookup.TryGetValue(def.Id, out var earnedAt) ? earnedAt : null
            }).ToList();
        }
    }
}