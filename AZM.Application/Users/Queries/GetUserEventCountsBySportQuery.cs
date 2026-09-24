using AZM.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Queries
{
    public record GetUserEventCountsBySportQuery(Guid UserId) : IRequest<List<SportEventCountDto>>;
}
