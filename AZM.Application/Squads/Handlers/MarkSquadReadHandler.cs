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
    public class MarkSquadReadHandler : IRequestHandler<MarkSquadReadCommand, Result<bool>>
    {
        private readonly ISquadRepository _squads;
        public MarkSquadReadHandler(ISquadRepository squads) => _squads = squads;

        public async Task<Result<bool>> Handle(MarkSquadReadCommand cmd, CancellationToken ct)
        {
            var member = await _squads.GetMemberAsync(cmd.SquadId, cmd.UserId, ct);
            if (member is null || member.Status != SquadMemberStatus.Approved)
                return Result<bool>.Failure("Not a member of this squad.", 403);

            member.MarkRead();
            await _squads.UpdateMemberAsync(member, ct);
            return Result<bool>.Success(true);
        }
    }
}
