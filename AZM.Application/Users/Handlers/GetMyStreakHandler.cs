using AZM.Application.DTOs.User;
using AZM.Application.Users.Queries;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Users.Handlers
{
    public class GetMyStreakHandler : IRequestHandler<GetMyStreakQuery, StreakDto>
    {
        private static readonly int[] Milestones = { 3, 7, 14, 30, 100 };
        private readonly IUserRepository _userRepository;

        public GetMyStreakHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<StreakDto> Handle(GetMyStreakQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(request.UserId)
                ?? throw new KeyNotFoundException("User not found.");

            var profile = user.Profile
                ?? throw new InvalidOperationException("User has no profile.");

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var nextMilestone = Milestones.FirstOrDefault(m => m > profile.CurrentStreak, 0);

            return new StreakDto
            {
                CurrentStreak = profile.CurrentStreak,
                LongestStreak = profile.LongestStreak,
                FreezesAvailable = profile.StreakFreezesAvailable,
                NextMilestone = nextMilestone,
                LastActiveDate = profile.LastActiveDate,
                IsAtRiskToday = profile.LastActiveDate.HasValue
                    && profile.LastActiveDate.Value == today.AddDays(-1)
            };
        }
    }
}
