using AZM.Application.Common;
using AZM.Application.DTOs.Event;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Queries
{
    public record GetUserEventsQuery(Guid UserId) : IRequest<Result<IEnumerable<EventFeedItemDto>>>;
}
