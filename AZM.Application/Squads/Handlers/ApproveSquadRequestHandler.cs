using AZM.Application.Common;
using AZM.Application.Squad.Commands;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
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

        public ApproveSquadRequestHandler(ISquadRepository squadRepo) => _squadRepo = squadRepo;

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

            return Result<bool>.Success(true);
        }
    }
}
