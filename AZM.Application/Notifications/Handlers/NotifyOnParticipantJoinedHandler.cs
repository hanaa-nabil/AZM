using AZM.Domain.DomainEvents;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Notifications.Handler
{
    public class NotifyOnParticipantJoinedHandler : INotificationHandler<EventParticipantJoined>
    {
        private readonly INotificationService _notifications;
        private readonly IUserRepository _userRepository;

        public NotifyOnParticipantJoinedHandler(INotificationService notifications, IUserRepository userRepository)
        {
            _notifications = notifications;
            _userRepository = userRepository;
        }

        public async Task Handle(EventParticipantJoined e, CancellationToken cancellationToken)
        {
            var participant = await _userRepository.GetByIdAsync(e.ParticipantId.ToString());

            await _notifications.SendAsync(
               e.OrganizerId,
               NotificationType.ParticipantJoined,
               "New participant",
               $"{participant?.FullName ?? "Someone"} joined your event.",
               e.EventId,
               actorId: e.ParticipantId,
               cancellationToken);
        }
    }
}