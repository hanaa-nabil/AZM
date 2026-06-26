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
    public record GetOrganizerEventsQuery(Guid OrganizerId, Guid? RequestingUserId)
        : IRequest<Result<IEnumerable<EventFeedItemDto>>>;

}

