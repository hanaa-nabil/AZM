using AZM.Application.Common;
using AZM.Application.Squads.Commands;
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
    public class RemoveSquadMemberHandler : IRequestHandler<RemoveSquadMemberCommand, Result<bool>>
    {
        private readonly ISquadRepository _squadRepo;

        public RemoveSquadMemberHandler(ISquadRepository squadRepo) => _squadRepo = squadRepo;

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

            return Result<bool>.Success(true);
        }
    }
}
