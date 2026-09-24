using AZM.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Commands
{
    public record RepublishEventCommand(Guid EventId, Guid RequestingUserId) : IRequest<Result<bool>>;
}
