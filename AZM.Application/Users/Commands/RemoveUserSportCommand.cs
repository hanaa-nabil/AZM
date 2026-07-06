using AZM.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Commands
{
    public record RemoveUserSportCommand(Guid UserId, Sport Sport) : IRequest<Unit>;
}
