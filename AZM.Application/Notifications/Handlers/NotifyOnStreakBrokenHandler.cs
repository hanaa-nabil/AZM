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
    public class NotifyOnStreakBrokenHandler : INotificationHandler<StreakBrokenEvent>
    {
        private readonly INotificationService _notificationService;

        public NotifyOnStreakBrokenHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(StreakBrokenEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.SendAsync(
                notification.UserId,
                NotificationType.StreakBroken,
                "Streak reset",
                $"Your {notification.PreviousStreak}-day streak ended — start a new one today!",
                ct: cancellationToken);
        }
    }
}