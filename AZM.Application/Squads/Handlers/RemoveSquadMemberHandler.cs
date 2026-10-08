using AZM.Application.Common;
using AZM.Application.Squads.Commands;
using AZM.Domain.Entities;
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
    public class RemoveSquadMemberHandler : IRequestHandler<RemoveSquadMemberCommand, Result<bool>>
    {
        private readonly ISquadRepository _squadRepo;
        private readonly INotificationService _notifications;
        private readonly ILogger<RemoveSquadMemberHandler> _logger;

        public RemoveSquadMemberHandler(ISquadRepository squadRepo, INotificationService notifications, ILogger<RemoveSquadMemberHandler> logger)
        {
            _squadRepo = squadRepo;
            _notifications = notifications;
            _logger = logger;
        }
        public async Task<Result<bool>> Handle(RemoveSquadMemberCommand cmd, CancellationToken ct)
        {
            var requester = await _squadRepo.GetMemberAsync(cmd.SquadId, cmd.RequestingUserId, ct);
            if (requester is null || requester.Role == SquadMemberRole.Member)
                return Result<bool>.Failure("Only the founder or a co-captain can remove members.", 403);

            var target = await _squadRepo.GetMemberAsync(cmd.SquadId, cmd.TargetUserId, ct);
            if (target is null)
                return Result<bool>.Failure("Member not found.", 404);

            if (target.Role == SquadMemberRole.Founder)
                return Result<bool>.Failure("The founder cannot be removed.");

            target.Remove();
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
