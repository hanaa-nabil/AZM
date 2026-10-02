using AZM.Application.Common;
using AZM.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Squad.Commands
{
    public record CreateSquadCommand(
    Guid FounderId, string Name, string? Bio, List<SportType> Sports,
    double? Latitude, double? Longitude, string? LocationName,
    SquadPrivacy Privacy, string? CoverImageUrl) : IRequest<Result<Guid>>;
}
