using AZM.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Follows.Commands
{
    public record UnfollowUserByUsernameCommand(Guid FollowerId, string Username) : IRequest<Result>;
}
