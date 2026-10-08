using AZM.Application.Common;
using AZM.Application.DTOs.Squad;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Squads.Commands
{
    public record SendSquadMessageCommand(Guid SquadId, Guid SenderId, string Content) : IRequest<Result<SquadMessageDto>>;
  
}
