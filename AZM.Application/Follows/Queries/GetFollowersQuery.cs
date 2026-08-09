using AZM.Application.DTOs.Follow;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Follows.Queries
{
    public record GetFollowersQuery(Guid UserId) : IRequest<List<FollowUserDto>>;
}
