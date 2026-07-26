using AZM.Domain.DomainEvents;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Handlers
{
    public class AwardStreakBadgeHandler : INotificationHandler<StreakMilestoneReachedEvent>
    {
        private readonly IAchievementRepository _achievementRepository;
        private readonly IMediator _mediator;

        public AwardStreakBadgeHandler(IAchievementRepository achievementRepository, IMediator mediator)
        {
            _achievementRepository = achievementRepository;
            _mediator = mediator;
        }

        public async Task Handle(StreakMilestoneReachedEvent notification, CancellationToken cancellationToken)
        {
            var definition = await _achievementRepository.GetByCriteriaAsync(
                AchievementCriteriaType.StreakDays, notification.StreakCount);

            if (definition is null)
                return; // no badge configured for this milestone number

            if (await _achievementRepository.AlreadyEarnedAsync(notification.UserId, definition.Id))
                return; // idempotency guard — safe if the event is ever redelivered

            await _achievementRepository.GrantAsync(notification.UserId, definition.Id);

            await _mediator.Publish(
                new BadgeEarnedEvent(notification.UserId, definition.Id, definition.Name),
                cancellationToken);
        }
    }
}
