using AZM.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Squad.Commands
{
    public record RequestToJoinSquadCommand(Guid SquadId, Guid UserId) : IRequest<Result<bool>>;
}
