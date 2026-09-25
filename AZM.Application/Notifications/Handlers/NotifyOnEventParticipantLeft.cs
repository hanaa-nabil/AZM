using AZM.Domain.DomainEvents;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Notifications.Handler
{
    public class NotifyOnEventParticipantLeft : INotificationHandler<EventParticipantLeft>
    {
        private readonly INotificationService _notifications;
        private readonly IUserRepository _userRepository;

        public NotifyOnEventParticipantLeft(INotificationService notifications, IUserRepository userRepository)
        {
            _notifications = notifications;
            _userRepository = userRepository;
        }

        public async Task Handle(EventParticipantLeft e, CancellationToken cancellationToken)
        {
            var participant = await _userRepository.GetByIdAsync(e.ParticipantId.ToString());

            await _notifications.SendAsync(
                e.OrganizerId,
                NotificationType.ParticipantLeft,
                "Participant left",
                $"{participant?.FullName ?? "Someone"} left your event.",
                e.EventId,
                actorId: e.ParticipantId,
                cancellationToken);
        }
    }
}