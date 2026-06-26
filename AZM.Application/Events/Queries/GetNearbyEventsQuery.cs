using AZM.Application.Auth.DTOs.Event;
using AZM.Application.Common;
using AZM.Domain.Enums;
using MediatR;

namespace AZM.Application.Events.Queries
{
    public record GetNearbyEventsQuery(
       double Latitude,
       double Longitude,
       double RadiusKm = 10,
       Guid? RequestingUserId = null
   ) : IRequest<Result<IEnumerable<EventFeedItemDto>>>;
}