using AZM.Domain.DomainEvents;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Notifications.Handler
{
    public class NotifyOnParticipantJoinedHandler : INotificationHandler<EventParticipantJoined>
    {
        private readonly INotificationService _notifications;

        public NotifyOnParticipantJoinedHandler(INotificationService notifications)
            => _notifications = notifications;

        public Task Handle(EventParticipantJoined e, CancellationToken cancellationToken)
            => _notifications.SendAsync(
                e.OrganizerId,
                NotificationType.ParticipantJoined,
                "New participant",
                "Someone joined your event",
                e.EventId,
                cancellationToken);
    }
}
