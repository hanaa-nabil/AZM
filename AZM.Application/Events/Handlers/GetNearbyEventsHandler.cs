
using AZM.Application.Common;
using AZM.Application.DTOs.Event;
using AZM.Application.Events.Queries;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class GetNearbyEventsHandler : IRequestHandler<GetNearbyEventsQuery, Result<IEnumerable<EventFeedItemDto>>>
    {
        private readonly IEventRepository _eventRepo;

        public GetNearbyEventsHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<IEnumerable<EventFeedItemDto>>> Handle(GetNearbyEventsQuery q, CancellationToken ct)
        {
            var events = await _eventRepo.GetNearbyAsync(q.Latitude, q.Longitude, q.RadiusKm, ct);

            HashSet<Guid> joinedIds = [];
            if (q.RequestingUserId.HasValue)
            {
                var joined = await _eventRepo.GetUserJoinedEventsAsync(q.RequestingUserId.Value, ct);
                joinedIds = joined.Select(e => e.Id).ToHashSet();
            }

            var items = events.Select(e => new EventFeedItemDto
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                SportType = e.SportType.ToString(),
                DifficultyLevel = e.DifficultyLevel.ToString(),
                Status = e.Status.ToString(),
                LocationName = e.LocationName,
                Latitude = e.Latitude,
                Longitude = e.Longitude,
                EventDate = e.EventDate,
                CreatedAt = e.CreatedAt,
                ParticipantCount = e.ParticipantCount,
                MaxParticipants = e.MaxParticipants,
                IsFull = e.IsFull,
                DistanceKm = e.DistanceKm,
                CoverImageUrl = e.CoverImageUrl,
                Organizer = new OrganizerSummaryDto
                {
                    Id = e.OrganizerId,
                    FullName = $"{e.Organizer.FirstName} {e.Organizer.LastName}".Trim(),
                },
                IsJoined = joinedIds.Contains(e.Id)
            });

            return Result<IEnumerable<EventFeedItemDto>>.Success(items);
        }
    }

}