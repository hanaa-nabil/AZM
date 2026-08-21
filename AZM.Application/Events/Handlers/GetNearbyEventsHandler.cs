
using AZM.Application.Common;
using AZM.Application.DTOs.Event;
using AZM.Application.Events.Queries;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class GetNearbyEventsHandler : IRequestHandler<GetNearbyEventsQuery, Result<IEnumerable<NearbyEventDto>>>
    {
        private static readonly EventStatus[] VisibleStatuses =
        {
            EventStatus.Scheduled,
            EventStatus.Ongoing,
            EventStatus.Active,
            EventStatus.Upcoming,
            EventStatus.Published
        };

        private readonly IEventRepository _eventRepo;
        public GetNearbyEventsHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<IEnumerable<NearbyEventDto>>> Handle(GetNearbyEventsQuery q, CancellationToken ct)
        {
            var events = await _eventRepo.GetNearbyAsync(q.Latitude, q.Longitude, q.RadiusKm, ct);

            var items = events
                .Where(e => VisibleStatuses.Contains(e.Status))
                .Select(e => new NearbyEventDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    SportType = e.SportType.ToString(),
                    Latitude = e.Latitude,
                    Longitude = e.Longitude
                });

            return Result<IEnumerable<NearbyEventDto>>.Success(items);
        }
    }
}