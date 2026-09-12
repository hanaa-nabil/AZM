using AZM.Application.Common;
using AZM.Application.DTOs.Event;
using AZM.Application.DTOs.Participants;
using AZM.Application.Events.Queries;
using AZM.Domain.Interfaces;
using MediatR;
namespace AZM.Application.Events.Handlers
{
    public class GetMyJoinedEventsHandler : IRequestHandler<GetMyJoinedEventsQuery, Result<IEnumerable<EventFeedItemDto>>>
    {
        private readonly IEventRepository _eventRepo;

        public GetMyJoinedEventsHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<IEnumerable<EventFeedItemDto>>> Handle(GetMyJoinedEventsQuery q, CancellationToken ct)
        {
            var events = await _eventRepo.GetUserJoinedEventsAsync(q.UserId, ct);

            var items = new List<EventFeedItemDto>();
            foreach (var e in events)
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
                    DistanceKm = e.DistanceKm,
                    CoverImageUrl = e.CoverImageUrl,
                    Organizer = new OrganizerSummaryDto
                    {
                        Id = e.OrganizerId,
                        FullName = $"{e.Organizer.FirstName} {e.Organizer.LastName}".Trim(),
                    },
                    //Participants = participants.Select(p => new ParticipantDto
                    //{
                    //    Id = p.UserId,
                    //    FullName = $"{p.User.FirstName} {p.User.LastName}".Trim(),
                    //    AvatarUrl = p.User.ProfilePhotoUrl
                    //}).ToList(),

                    Participants =participants
                    .Where(p => p.Status == Domain.Enums.ParticipantStatus.Joined)
                    .Select(p => new ParticipantDto
                    {
                        Id = p.UserId,
                        FullName = $"{p.User.FirstName} {p.User.LastName}".Trim(),
                        AvatarUrl = p.User.ProfilePhotoUrl,
                        IsVerified = p.User.IsIdVerified && p.User.IsFaceVerified,
                        JoinedAt = p.JoinedAt,
                        Status = p.Status.ToString()
                    }).ToList(),
                    IsJoined = true,
                    Pace = e.Pace,
                    IsOrganizer = false,
                });
            }

            return Result<IEnumerable<EventFeedItemDto>>.Success(items);
        }
    }
}
