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

    public class DeleteSquadMessageHandler : IRequestHandler<DeleteSquadMessageCommand, Result<Guid>>
    {
        private readonly ISquadRepository _squads;
        private readonly ISquadMessageRepository _messages;
        public DeleteSquadMessageHandler(ISquadRepository squads, ISquadMessageRepository messages)
        { _squads = squads; _messages = messages; }

        public async Task<Result<Guid>> Handle(DeleteSquadMessageCommand cmd, CancellationToken ct)
        {
            var message = await _messages.GetByIdAsync(cmd.MessageId, ct);
            if (message is null) return Result<Guid>.Failure("Message not found.", 404);

            var member = await _squads.GetMemberAsync(message.SquadId, cmd.RequestingUserId, ct);
            var isStaff = member is { Status: SquadMemberStatus.Approved } && member.Role != SquadMemberRole.Member;
            if (message.SenderId != cmd.RequestingUserId && !isStaff)
                return Result<Guid>.Failure("You can only delete your own messages.", 403);

            message.SoftDelete();
            await _messages.UpdateAsync(message, ct);
            return Result<Guid>.Success(message.SquadId);
        }
    }
}
