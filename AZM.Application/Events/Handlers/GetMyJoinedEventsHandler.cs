using AZM.Application.Auth.DTOs.Event;
using AZM.Application.Common;
using AZM.Application.Events.Queries;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Handlers
{

    public class GetMyJoinedEventsHandler : IRequestHandler<GetMyJoinedEventsQuery, Result<IEnumerable<EventFeedItemDto>>>
    {
        private readonly IEventRepository _eventRepo;

        public GetMyJoinedEventsHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<IEnumerable<EventFeedItemDto>>> Handle(GetMyJoinedEventsQuery q, CancellationToken ct)
        {
            var events = await _eventRepo.GetUserJoinedEventsAsync(q.UserId, ct);

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
                IsJoined = true
            });

            return Result<IEnumerable<EventFeedItemDto>>.Success(items);
        }
    }
}
