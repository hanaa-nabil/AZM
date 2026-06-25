using AZM.Application.Auth.DTOs.Event;
using AZM.Domain.Enums;
using MediatR;

namespace AZM.Application.Events.Queries
{
    public class GetNearbyEventsQuery : IRequest<List<EventSummaryDto>>
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        public int RadiusMeters { get; set; } = 10000;
        public SportType? SportType { get; set; }
    }
}