using AZM.Application.Common;
using AZM.Application.DTOs.Squad;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Squads.Queries
{
    public record GetSquadRosterQuery(Guid SquadId) : IRequest<Result<SquadDetailDto>>;
}
