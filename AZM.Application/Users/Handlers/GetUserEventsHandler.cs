using AZM.Application.Common;
using AZM.Application.DTOs.Event;
using AZM.Application.DTOs.Participants;
using AZM.Application.Events.Queries;
using AZM.Application.Users.Queries;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class GetUserEventsHandler : IRequestHandler<GetUserEventsQuery, Result<IEnumerable<EventFeedItemDto>>>
    {
        private readonly IEventRepository _eventRepo;

        public GetUserEventsHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<IEnumerable<EventFeedItemDto>>> Handle(GetUserEventsQuery request, CancellationToken ct)
        {
            var joined = await _eventRepo.GetUserJoinedEventsAsync(request.UserId, ct);
            var organized = await _eventRepo.GetByOrganizerAsync(request.UserId, ct);

            var combined = joined.Concat(organized)
                .Where(e => e.IsPublic)
                .DistinctBy(e => e.Id)
                .OrderByDescending(e => e.EventDate)
                .ToList();

            var items = new List<EventFeedItemDto>();

            foreach (var e in combined)
            {
                var participants = await _eventRepo.GetParticipantsAsync(e.Id, ct);

                items.Add(new EventFeedItemDto
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
                    CoverImageUrl = e.CoverImageUrl,
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
                            Username = p.User.UserName ?? string.Empty,
                            AvatarUrl = p.User.ProfilePhotoUrl,
                            IsVerified = p.User.IsIdVerified && p.User.IsFaceVerified,
                            JoinedAt = p.JoinedAt,
                            Status = p.Status.ToString()
                        }).ToList(),
                    Pace = e.Pace,
                    IsOrganizer = e.OrganizerId == request.UserId,
                    IsJoined = participants.Any(p => p.UserId == request.UserId && p.Status == ParticipantStatus.Joined)
                });
            }

            return Result<IEnumerable<EventFeedItemDto>>.Success(items);
        }
    }
}