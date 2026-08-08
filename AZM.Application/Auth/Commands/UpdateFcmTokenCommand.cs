using AZM.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Auth.Commands
{
    public record UpdateFcmTokenCommand(Guid UserId, string FcmToken) : IRequest<Result>;
}
