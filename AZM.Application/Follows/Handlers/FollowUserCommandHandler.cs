using AZM.Application.Common;
using AZM.Application.Follows.Commands;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Follows.Handlers
{
    public class FollowUserCommandHandler : IRequestHandler<FollowUserCommand, Result>
    {
        private readonly IFollowRepository _followRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;

        public FollowUserCommandHandler(
            IFollowRepository followRepository,
            IUserRepository userRepository,
            INotificationService notificationService)
        {
            _followRepository = followRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public async Task<Result> Handle(FollowUserCommand request, CancellationToken cancellationToken)
        {
            if (request.FollowerId == request.FollowingId)
                return Result.Failure("You can't follow yourself.", 400);

            var targetUser = await _userRepository.GetByIdAsync(request.FollowingId.ToString());
            if (targetUser is null)
                return Result.Failure("User not found.", 404);

            var alreadyFollowing = await _followRepository.IsFollowingAsync(request.FollowerId, request.FollowingId);
            if (alreadyFollowing)
                return Result.Failure("You're already following this user.", 409);

            await _followRepository.FollowAsync(request.FollowerId, request.FollowingId);

            var follower = await _userRepository.GetByIdAsync(request.FollowerId.ToString());
            await _notificationService.SendAsync(
                request.FollowingId,
                NotificationType.NewFollower,
                "New follower",
                $"{follower?.FullName ?? "Someone"} started following you.",
                ct: cancellationToken);

            return Result.Success();
        }
    }
}
