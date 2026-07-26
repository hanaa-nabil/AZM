using AZM.Domain.DomainEvents;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Notifications.Handlers
{
    public class NotifyOnBadgeEarnedHandler : INotificationHandler<BadgeEarnedEvent>
    {
        private readonly INotificationService _notificationService;

        public NotifyOnBadgeEarnedHandler(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task Handle(BadgeEarnedEvent notification, CancellationToken cancellationToken)
        {
            await _notificationService.SendAsync(
                notification.UserId,
                NotificationType.BadgeEarned,
                "New badge unlocked!",
                $"You've earned the \"{notification.BadgeName}\" badge.",
                ct: cancellationToken);
        }
    }
}
