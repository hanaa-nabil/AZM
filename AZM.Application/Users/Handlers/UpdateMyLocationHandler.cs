using AZM.Application.Common;
using AZM.Application.Users.Commands;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Handlers
{
    public class UpdateMyLocationHandler : IRequestHandler<UpdateMyLocationCommand, Result<bool>>
    {
        private const double RadiusKm = 1.0;
        private static readonly TimeSpan FreshFor = TimeSpan.FromMinutes(30);
        private static readonly TimeSpan Cooldown = TimeSpan.FromHours(6);

        private readonly IUserRepository _users;
        private readonly IFollowRepository _follows;
        private readonly ISquadRepository _squads;
        private readonly INotificationRepository _notificationRepo;
        private readonly INotificationService _notifications;
        private readonly ILogger<UpdateMyLocationHandler> _logger;

        public UpdateMyLocationHandler(IUserRepository users, IFollowRepository follows,
            ISquadRepository squads, INotificationRepository notificationRepo,
            INotificationService notifications, ILogger<UpdateMyLocationHandler> logger)
        {
            _users = users; _follows = follows; _squads = squads;
            _notificationRepo = notificationRepo; _notifications = notifications; _logger = logger;
        }

        public async Task<Result<bool>> Handle(UpdateMyLocationCommand cmd, CancellationToken ct)
        {
            if (cmd.Latitude is < -90 or > 90 || cmd.Longitude is < -180 or > 180)
                return Result<bool>.Failure("Invalid coordinates.", 400);

            var me = await _users.GetByIdWithDetailsAsync(cmd.UserId);
            if (me?.Profile is null) return Result<bool>.Failure("Profile not found.", 404);
            if (!me.Profile.ShareLocation)
                return Result<bool>.Failure("Turn on location sharing first.", 400);

            me.Profile.UpdateLocation(cmd.Latitude, cmd.Longitude);
            await _users.UpdateAsync(me);

            try
            {
                var followerIds = await _follows.GetFollowerIdsAsync(cmd.UserId);
                var following = await _follows.GetFollowingAsync(cmd.UserId);
                var squadMates = await _squads.GetCoMemberIdsAsync(cmd.UserId, ct);

                var circle = followerIds
                    .Concat(following.Select(u => u.Id))
                    .Concat(squadMates)
                    .Distinct().ToList();
                if (circle.Count == 0) return Result<bool>.Success(true);

                var nearby = await _users.GetNearbyProfilesAsync(
                    circle, cmd.Latitude, cmd.Longitude, RadiusKm, DateTime.UtcNow - FreshFor, ct);

                var since = DateTime.UtcNow - Cooldown;
                foreach (var other in nearby)
                {
                    if (!await _notificationRepo.ExistsRecentAsync(cmd.UserId, other.UserId,
                            NotificationType.NearbyFriend, since, ct))
                        await _notifications.SendAsync(cmd.UserId, NotificationType.NearbyFriend,
                            "Friend nearby", $"{other.User!.FullName} is near you.",
                            actorId: other.UserId, ct: ct);

                    if (!await _notificationRepo.ExistsRecentAsync(other.UserId, cmd.UserId,
                            NotificationType.NearbyFriend, since, ct))
                        await _notifications.SendAsync(other.UserId, NotificationType.NearbyFriend,
                            "Friend nearby", $"{me.FullName} is near you.",
                            actorId: cmd.UserId, ct: ct);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Nearby-friend check failed for {UserId}.", cmd.UserId);
            }

            return Result<bool>.Success(true);
        }
    }
}
