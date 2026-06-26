using AZM.Application.Auth.DTOs.Event;
using AZM.Application.Common;
using AZM.Application.Events.Queries;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Handlers
{
    public class GetOrganizerEventsQueryHandler
       : IRequestHandler<GetOrganizerEventsQuery, Result<IEnumerable<EventFeedItemDto>>>
    {
        private readonly IEventRepository _eventRepo;

        public GetOrganizerEventsQueryHandler(IEventRepository eventRepo)
            => _eventRepo = eventRepo;

        public async Task<Result<IEnumerable<EventFeedItemDto>>> Handle(
            GetOrganizerEventsQuery request, CancellationToken ct)
        {
            var events = await _eventRepo.GetByOrganizerAsync(request.OrganizerId, ct);

            HashSet<Guid> joinedIds = [];
            if (request.RequestingUserId.HasValue)
            {
                var joined = await _eventRepo.GetUserJoinedEventsAsync(
                    request.RequestingUserId.Value, ct);
                joinedIds = joined.Select(e => e.Id).ToHashSet();
            }

            var items = events.Select(e => MapToFeedItem(e, joinedIds.Contains(e.Id)));
            return Result<IEnumerable<EventFeedItemDto>>.Success(items);
        }

        private static EventFeedItemDto MapToFeedItem(Event e, bool isJoined) => new()
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
                AvatarUrl = null
            },
            IsJoined = isJoined
        };
    }
}
