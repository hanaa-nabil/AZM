using AZM.Application.Common;
using AZM.Application.DTOs.Event;
using AZM.Application.DTOs.Participants;
using AZM.Application.Events.Queries;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Handlers
{
    public class SearchEventsHandler : IRequestHandler<SearchEventsQuery, Result<IEnumerable<EventFeedItemDto>>>
    {
        private readonly IEventRepository _eventRepo;
        public SearchEventsHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<IEnumerable<EventFeedItemDto>>> Handle(SearchEventsQuery q, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(q.SearchTerm))
                return Result<IEnumerable<EventFeedItemDto>>.Success(new List<EventFeedItemDto>());

            var (events, _) = await _eventRepo.SearchAsync(q.SearchTerm.Trim(), q.Page, q.PageSize, ct);

            var eventIds = events.Select(e => e.Id).ToList();
            var participantsByEvent = await _eventRepo.GetParticipantsForEventsAsync(eventIds, ct);

            HashSet<Guid> joinedIds = new();
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
                Status = e.DisplayStatus.ToString(),
                LocationName = e.LocationName,
                Latitude = e.Latitude,
                Longitude = e.Longitude,
                EventDate = e.EventDate,
                CreatedAt = e.CreatedAt,
                ParticipantCount = e.ParticipantCount,
                MaxParticipants = e.MaxParticipants,
                IsFull = e.IsFull,
                CoverImageUrl = e.CoverImageUrl,
                Pace = e.Pace,
                Organizer = new OrganizerSummaryDto
                {
                    Id = e.OrganizerId,
                    FullName = $"{e.Organizer.FirstName} {e.Organizer.LastName}".Trim(),
                    Username = e.Organizer.UserName ?? string.Empty,
                    AvatarUrl = e.Organizer.ProfilePhotoUrl
                },
                Participants = (participantsByEvent.TryGetValue(e.Id, out var list) ? list : new())
                    .Where(p => p.Status == ParticipantStatus.Joined)
                    .Select(p => new ParticipantDto
                    {
                        Id = p.UserId,
                        FullName = $"{p.User.FirstName} {p.User.LastName}".Trim(),
                        Username = p.User.UserName ?? string.Empty,
                        AvatarUrl = p.User.ProfilePhotoUrl,
                        IsVerified = p.User.IsIdVerified && p.User.IsFaceVerified,
                        JoinedAt = p.JoinedAt,
                        Status = p.Status.ToString()
                    }).ToList(),
                IsJoined = joinedIds.Contains(e.Id) || (q.RequestingUserId.HasValue && e.OrganizerId == q.RequestingUserId.Value),
                IsOrganizer = q.RequestingUserId.HasValue && e.OrganizerId == q.RequestingUserId.Value
            });

            return Result<IEnumerable<EventFeedItemDto>>.Success(items);
        }
    }
}
