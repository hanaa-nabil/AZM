using AZM.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Commands
{
    public record SetLocationSharingCommand(Guid UserId, bool Enabled) : IRequest<Result<bool>>;
}
