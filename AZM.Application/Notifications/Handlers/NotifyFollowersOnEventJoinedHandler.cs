using AZM.Domain.DomainEvents;
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
    public class NotifyFollowersOnEventJoinedHandler : INotificationHandler<EventParticipantJoined>
    {
        private readonly IFollowRepository _followRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public NotifyFollowersOnEventJoinedHandler(
            IFollowRepository followRepository,
            IUserRepository userRepository,
            INotificationService notificationService)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task Handle(EventParticipantJoined notification, CancellationToken cancellationToken)
        {
            // Notify the PARTICIPANT's followers, not the organizer's — this is
            // "your friend joined an event," distinct from EventPublished above
            // which is "your friend created an event."
            var followerIds = await _followRepository.GetFollowerIdsAsync(notification.ParticipantId);
            if (followerIds.Count == 0)
                return;

            var participant = await _userRepository.GetByIdAsync(notification.ParticipantId.ToString());
            var name = participant?.FullName ?? "Someone you follow";

            await _notificationService.SendBulkAsync(
                followerIds,
                NotificationType.FollowedUserJoinedEvent,
                "Friend joined an event",
                $"{name} just joined an event.",
                relatedEventId: notification.EventId,
                ct: cancellationToken);
        }
    }
}
