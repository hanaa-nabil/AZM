using AZM.Application.Auth.DTOs.Event;
using AZM.Application.Events.Queries;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class GetNearbyEventsHandler : IRequestHandler<GetNearbyEventsQuery, List<EventSummaryDto>>
    {
        private readonly IEventRepository _eventRepo;

        public GetNearbyEventsHandler(IEventRepository eventRepo)
        {
            _eventRepo = eventRepo;
        }

        public async Task<List<EventSummaryDto>> Handle(GetNearbyEventsQuery query, CancellationToken ct)
        {
            var events = await _eventRepo.GetNearbyAsync(
                query.Lat,
                query.Lng,
                query.RadiusMeters,
                query.SportType);

            return events.Select(e => new EventSummaryDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Difficulty = e.Difficulty,
                Status = e.Status,
                StartAtUtc = e.StartAtUtc,
                SportType = e.SportType,
                CreatedByUserId = e.CreatedByUserId,
                CreatedByName = e.CreatedByUser is not null
                    ? $"{e.CreatedByUser.FirstName} {e.CreatedByUser.LastName}"
                    : string.Empty,
                MeetingLat = e.MeetingLat,
                MeetingLng = e.MeetingLng,
                MeetingAddress = e.MeetingAddress,
                MaxParticipants = e.MaxParticipants,
                ParticipantCount = e.Participants.Count
            }).ToList();
        }
    }
}