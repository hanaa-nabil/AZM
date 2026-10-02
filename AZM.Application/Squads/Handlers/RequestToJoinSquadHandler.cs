using AZM.Application.Common;
using AZM.Application.Squad.Commands;
using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Squad.Handlers
{
    public class RequestToJoinSquadHandler : IRequestHandler<RequestToJoinSquadCommand, Result<bool>>
    {
        private readonly ISquadRepository _squadRepo;

        public RequestToJoinSquadHandler(ISquadRepository squadRepo) => _squadRepo = squadRepo;

        public async Task<Result<bool>> Handle(RequestToJoinSquadCommand cmd, CancellationToken ct)
        {
            var squad = await _squadRepo.GetByIdAsync(cmd.SquadId, ct);
            if (squad is null) return Result<bool>.Failure("Squad not found.", 404);

            var existing = await _squadRepo.GetMemberAsync(cmd.SquadId, cmd.UserId, ct);
            if (existing is not null && existing.Status != SquadMemberStatus.Removed)
                return Result<bool>.Failure(
                    existing.Status == SquadMemberStatus.Pending
                        ? "Your request is already pending."
                        : "You're already a member of this squad.");

            var autoApprove = squad.Privacy == SquadPrivacy.Public;
            var member = SquadMember.RequestToJoin(cmd.SquadId, cmd.UserId, autoApprove);
            await _squadRepo.AddMemberAsync(member, ct);

            return Result<bool>.Success(true);
        }
    }
}
