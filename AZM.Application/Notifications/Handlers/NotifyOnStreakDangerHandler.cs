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
    public class NotifyOnStreakDangerHandler : INotificationHandler<StreakDangerEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationService _notificationService;

        public NotifyOnStreakDangerHandler(
            INotificationRepository notificationRepository,
            INotificationService notificationService)
        {
            _notificationRepository = notificationRepository;
            _notificationService = notificationService;
        }

        public async Task Handle(StreakDangerEvent notification, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var alreadySent = await _notificationRepository.ExistsForDateAsync(
                notification.UserId, NotificationType.StreakDanger, today);
            if (alreadySent)
                return;

            await _notificationService.SendAsync(
                notification.UserId,
                NotificationType.StreakDanger,
                "Streak danger!",
                $"Your {notification.CurrentStreak}-day streak expires soon. Complete a quick activity to save it!",
                ct: cancellationToken);
        }
    }
}
