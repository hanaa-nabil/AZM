using AZM.Application.Auth.DTOs.Event;
using AZM.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Queries
{
    public record GetMyJoinedEventsQuery(Guid UserId) : IRequest<Result<IEnumerable<EventFeedItemDto>>>;
}
