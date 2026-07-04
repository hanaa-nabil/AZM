using AZM.Application.Common;
using AZM.Application.DTOs.Event;
using MediatR;

namespace AZM.Application.Events.Queries
{
    public record GetEventByIdQuery(Guid EventId, Guid? RequestingUserId) : IRequest<Result<EventDetailDto>>;
}
