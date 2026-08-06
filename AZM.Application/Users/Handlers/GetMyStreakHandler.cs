using AZM.Application.DTOs.User;
using AZM.Application.Users.Queries;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Users.Handlers
{
    public class GetMyStreakHandler : IRequestHandler<GetMyStreakQuery, StreakDto>
    {
        private const int DaysToShow = 7; // matches a week-strip UI

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

            var recentActivity = await _userRepository.GetRecentActivityAsync(request.UserId, DaysToShow);
            var activityByDate = recentActivity.ToDictionary(a => a.Date, a => a.IsActive);

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var days = new List<StreakDayDto>();

            for (int i = DaysToShow - 1; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                days.Add(new StreakDayDto
                {
                    Date = date.ToString("yyyy-MM-dd"),
                    IsActive = activityByDate.TryGetValue(date, out var isActive) && isActive
                });
            }

            return new StreakDto
            {
                StreakCount = profile.CurrentStreak,
                FreezeCount = profile.StreakFreezesAvailable,
                Days = days
            };
        }
    }
}
