using AZM.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Commands
{
    public record CompleteEventActivityCommand(
    Guid UserId,
    Guid EventId,
    int Steps,
    double DistanceMeters
) : IRequest<Result<bool>>;
}
