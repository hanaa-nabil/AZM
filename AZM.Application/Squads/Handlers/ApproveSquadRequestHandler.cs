using AZM.Application.Common;
using AZM.Application.Squad.Commands;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Squads.Handlers
{
    public class ApproveSquadRequestHandler : IRequestHandler<ApproveSquadRequestCommand, Result<bool>>
    {
        private readonly ISquadRepository _squadRepo;
        private readonly INotificationService _notifications;
        private readonly ILogger<ApproveSquadRequestHandler> _logger;

        public ApproveSquadRequestHandler(ISquadRepository squadRepo, INotificationService notifications, ILogger<ApproveSquadRequestHandler> logger)
        {
            _squadRepo = squadRepo;
            _notifications = notifications;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(ApproveSquadRequestCommand cmd, CancellationToken ct)
        {
            var requester = await _squadRepo.GetMemberAsync(cmd.SquadId, cmd.RequestingUserId, ct);
            if (requester is null || requester.Role == SquadMemberRole.Member)
                return Result<bool>.Failure("Only the founder or a co-captain can approve requests.", 403);

            var target = await _squadRepo.GetMemberAsync(cmd.SquadId, cmd.TargetUserId, ct);
            if (target is null || target.Status != SquadMemberStatus.Pending)
                return Result<bool>.Failure("No pending request found for this user.", 404);

            target.Approve();
            await _squadRepo.UpdateMemberAsync(target, ct);
            try
            {
                var squad = await _squadRepo.GetByIdAsync(cmd.SquadId, ct);
                await _notifications.SendAsync(
                    cmd.TargetUserId,
                    NotificationType.SquadRequestApproved,
                    squad!.Name,
                    "Your request to join was approved.",
                    actorId: cmd.RequestingUserId,
                    squadId: cmd.SquadId,
                    ct: ct);
            }
            catch (Exception ex) { _logger.LogError(ex, "Approve notification failed."); }
            return Result<bool>.Success(true);
        }
    }
}
