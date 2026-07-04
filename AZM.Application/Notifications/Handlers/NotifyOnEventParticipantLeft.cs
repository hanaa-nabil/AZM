

using AZM.Domain.DomainEvents;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Notifications.Handler
{
    public class NotifyOnEventParticipantLeft : INotificationHandler<EventParticipantLeft>
    {
        private readonly INotificationService _notifications;

        public NotifyOnEventParticipantLeft(INotificationService notifications)
            => _notifications = notifications;

        public Task Handle(EventParticipantLeft e, CancellationToken cancellationToken)
            => _notifications.SendAsync(
                e.OrganizerId,
                NotificationType.ParticipantLeft,
                "Participant left",
                "Someone left your event",
                e.EventId,
                cancellationToken);
    }
}
