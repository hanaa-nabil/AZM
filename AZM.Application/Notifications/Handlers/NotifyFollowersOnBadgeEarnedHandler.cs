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
    public class NotifyFollowersOnBadgeEarnedHandler : INotificationHandler<BadgeEarnedEvent>
    {
        private readonly IFollowRepository _followRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public NotifyFollowersOnBadgeEarnedHandler(
            IFollowRepository followRepository,
            IUserRepository userRepository,
            INotificationService notificationService)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task Handle(BadgeEarnedEvent notification, CancellationToken cancellationToken)
        {
            var followerIds = await _followRepository.GetFollowerIdsAsync(notification.UserId);
            if (followerIds.Count == 0)
                return;

            var user = await _userRepository.GetByIdAsync(notification.UserId.ToString());
            var name = user?.FullName ?? "Someone you follow";

            await _notificationService.SendBulkAsync(
                followerIds,
                NotificationType.FollowedUserEarnedBadge,
                "Badge earned!",
                $"{name} just earned the \"{notification.BadgeName}\" badge.",
                ct: cancellationToken);
        }
    }
}
