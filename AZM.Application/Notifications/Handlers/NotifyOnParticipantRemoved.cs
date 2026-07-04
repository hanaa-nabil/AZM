using AZM.Domain.DomainEvents;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Notifications.Handler
{
    public class NotifyOnParticipantRemoved : INotificationHandler<ParticipantRemoved>
    {
        private readonly INotificationService _notifications;

        public NotifyOnParticipantRemoved(INotificationService notifications)
            => _notifications = notifications;

        public Task Handle(ParticipantRemoved e, CancellationToken cancellationToken)
            => _notifications.SendAsync(
                e.ParticipantId,
                NotificationType.RemovedFromEvent,
                "Removed from event",
                "You were removed from an event",
                e.EventId,
                cancellationToken);
    }
}
