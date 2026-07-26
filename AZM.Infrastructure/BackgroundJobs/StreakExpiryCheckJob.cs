using AZM.Domain.DomainEvents;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Infrastructure.BackgroundJobs
{
    public class StreakExpiryCheckJob
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public StreakExpiryCheckJob(IUserRepository userRepository, IMediator mediator)
        {
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task RunAsync()
        {
            var yesterday = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-1);
            var atRiskUsers = await _userRepository.GetUsersWithStreakLastActiveAsync(yesterday);

            var expiresAtUtc = DateTime.UtcNow.Date.AddDays(1); // midnight tonight, adjust to taste

            foreach (var user in atRiskUsers)
            {
                await _mediator.Publish(new StreakDangerEvent(user.UserId, user.CurrentStreak, expiresAtUtc));
            }
        }
    }
}
