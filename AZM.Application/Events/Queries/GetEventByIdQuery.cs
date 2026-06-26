using AZM.Application.Auth.DTOs.Event;
using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Events.Queries
{
    public record GetEventByIdQuery(Guid EventId, Guid? RequestingUserId) : IRequest<Result<EventDetailDto>>;
}
