
using AZM.Domain.DomainEvents;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Notifications.Handler
{
    public class NotifyOnEventUpdated : INotificationHandler<EventUpdated>
    {
        private readonly INotificationService _notifications;

        public NotifyOnEventUpdated(INotificationService notifications)
            => _notifications = notifications;

        public Task Handle(EventUpdated e, CancellationToken cancellationToken)
            => _notifications.SendBulkAsync(
                e.ParticipantIds,
                NotificationType.EventUpdated,
                "Event updated",
                "An event you joined has changed — check the details",
                e.EventId,
                cancellationToken);
    }
}
