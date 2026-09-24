using AZM.Application.Common;
using AZM.Application.DTOs.Event;
using AZM.Application.DTOs.Participants;
using AZM.Application.Events.Queries;
using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class GetEventFeedHandler : IRequestHandler<GetEventFeedQuery, Result<EventFeedResponseDto>>
    {
        private readonly IEventRepository _eventRepo;

        public GetEventFeedHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<EventFeedResponseDto>> Handle(GetEventFeedQuery q, CancellationToken ct)
        {
            var effectiveStatus = q.Status;
            var (events, total) = await _eventRepo.GetFeedAsync(q.Page, q.PageSize, q.SportType, effectiveStatus, ct);

            if (!q.Status.HasValue)
            {
                events = events.Where(e => e.Status != EventStatus.Completed).ToList();
                total = events.Count(); // note: this breaks accurate pagination totals, see note below
            }

            HashSet<Guid> joinedIds = [];
            if (q.RequestingUserId.HasValue)
            {
                var joined = await _eventRepo.GetUserJoinedEventsAsync(q.RequestingUserId.Value, ct);
                joinedIds = joined.Select(e => e.Id).ToHashSet();
            }

            var eventIds = events.Select(e => e.Id).ToList();
            var participantsByEvent = await _eventRepo.GetParticipantsForEventsAsync(eventIds, ct);

            var items = events.Select(e => MapToFeedItem(
                e,
                isJoined: joinedIds.Contains(e.Id),
                requestingUserId: q.RequestingUserId,
                participants: participantsByEvent.TryGetValue(e.Id, out var list) ? list : new List<EventParticipant>()));

            return Result<EventFeedResponseDto>.Success(new EventFeedResponseDto
            {
                Events = items,
                TotalCount = total,
                Page = q.Page,
                PageSize = q.PageSize,
                HasMore = q.Page * q.PageSize < total
            });
        }

        private static EventFeedItemDto MapToFeedItem(
            Event e, bool isJoined, Guid? requestingUserId, List<EventParticipant> participants) => new()
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
                Pace = e.Pace,
                Organizer = new OrganizerSummaryDto
                {
                    Id = e.OrganizerId,
                    FullName = $"{e.Organizer.FirstName} {e.Organizer.LastName}".Trim(),
                    Username = e.Organizer.UserName ?? string.Empty,
                    AvatarUrl = e.Organizer.ProfilePhotoUrl
                },
                Participants = participants
                .Where(p => p.Status == ParticipantStatus.Joined)
                .Select(p => new ParticipantDto
                {
                    Id = p.UserId,
                    FullName = $"{p.User.FirstName} {p.User.LastName}".Trim(),
                    AvatarUrl = p.User.ProfilePhotoUrl,
                    IsVerified = p.User.IsIdVerified && p.User.IsFaceVerified,
                    JoinedAt = p.JoinedAt,
                    Status = p.Status.ToString()
                }).ToList(),
                Route = e.Route is not null ? new EventRouteDto(
                        e.Route.StartLatitude, e.Route.StartLongitude, e.Route.StartAddress,
                        e.Route.EndLatitude, e.Route.EndLongitude, e.Route.EndAddress,
                        e.Route.DistanceMeters, e.Route.EstimatedDurationSeconds, e.Route.Polyline
                           ) : null,
                IsJoined = isJoined || (requestingUserId.HasValue && e.OrganizerId == requestingUserId.Value),
                IsOrganizer = requestingUserId.HasValue && e.OrganizerId == requestingUserId.Value
            };
    }
}