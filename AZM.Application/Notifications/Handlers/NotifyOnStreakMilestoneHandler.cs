using AZM.Domain.DomainEvents;
using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Notifications.Handlers
{
    public class NotifyOnStreakMilestoneHandler : INotificationHandler<StreakMilestoneReachedEvent>
    {
        private readonly INotificationService _notificationService;

        public NotifyOnStreakMilestoneHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(StreakMilestoneReachedEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.SendAsync(
                notification.UserId,
                NotificationType.StreakMilestone,
                "Streak milestone!",
                $"You've hit a {notification.StreakCount}-day streak. Keep it up!",
                ct: cancellationToken);
        }
    }
}

