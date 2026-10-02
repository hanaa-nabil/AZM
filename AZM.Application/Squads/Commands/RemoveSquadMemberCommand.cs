using AZM.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Squads.Commands
{
    public record RemoveSquadMemberCommand(Guid SquadId, Guid TargetUserId, Guid RequestingUserId) : IRequest<Result<bool>>;
}
