using AZM.Application.Common;
using AZM.Application.DTOs.Event;
using MediatR;

namespace AZM.Application.Events.Queries
{
    public record SearchEventsQuery(string SearchTerm, Guid? RequestingUserId, int Page = 1, int PageSize = 20)
    : IRequest<Result<IEnumerable<EventFeedItemDto>>>;
}
