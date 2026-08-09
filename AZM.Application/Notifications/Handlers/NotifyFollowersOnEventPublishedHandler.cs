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
    public class NotifyFollowersOnEventPublishedHandler : INotificationHandler<EventPublished>
    {
        private readonly IFollowRepository _followRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public NotifyFollowersOnEventPublishedHandler(
            IFollowRepository followRepository,
            IUserRepository userRepository,
            INotificationService notificationService)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task Handle(EventPublished notification, CancellationToken cancellationToken)
        {
            var followerIds = await _followRepository.GetFollowerIdsAsync(notification.OrganizerId);
            if (followerIds.Count == 0)
                return;

            var organizer = await _userRepository.GetByIdAsync(notification.OrganizerId.ToString());
            var name = organizer?.FullName ?? "Someone you follow";

            await _notificationService.SendBulkAsync(
                followerIds,
                NotificationType.FollowedUserPublishedEvent,
                "New event",
                $"{name} just published a new event.",
                relatedEventId: notification.EventId,
                ct: cancellationToken);
        }
    }
}
