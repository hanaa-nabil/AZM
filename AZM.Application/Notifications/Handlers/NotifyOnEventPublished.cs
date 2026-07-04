using AZM.Domain.DomainEvents;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Notifications.Handler
{
    public class NotifyOnEventPublished : INotificationHandler<EventPublished>
    {
        private readonly INotificationService _notifications;

        public NotifyOnEventPublished(INotificationService notifications)
            => _notifications = notifications;

        public Task Handle(EventPublished e, CancellationToken cancellationToken)
            => _notifications.SendAsync(
                e.OrganizerId,
                NotificationType.EventPublished,
                "Event published",
                "Your event is now live",
                e.EventId,
                cancellationToken);
    }
}
