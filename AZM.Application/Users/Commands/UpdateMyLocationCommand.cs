using AZM.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Commands
{
    public record UpdateMyLocationCommand(Guid UserId, double Latitude, double Longitude) : IRequest<Result<bool>>;
}
