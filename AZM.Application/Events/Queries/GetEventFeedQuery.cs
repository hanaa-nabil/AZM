using AZM.Application.Auth.DTOs.Event;
using AZM.Application.Common;
using AZM.Domain.Enums;
using MediatR;
namespace AZM.Application.Events.Queries
{
    public record GetEventFeedQuery(
     Guid? RequestingUserId,
     int Page = 1,
     int PageSize = 20,
     SportType? SportType = null,
     EventStatus? Status = null

 ) : IRequest<Result<EventFeedResponseDto>>;
}
