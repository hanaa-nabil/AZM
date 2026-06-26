using AZM.Application.Auth.DTOs.Event;
using AZM.Application.Auth.DTOs.Participants;
using AZM.Application.Common;
using AZM.Application.Events.Queries;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class GetEventByIdHandler : IRequestHandler<GetEventByIdQuery, Result<EventDetailDto>>
    {
        private readonly IEventRepository _eventRepo;

        public GetEventByIdHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<EventDetailDto>> Handle(GetEventByIdQuery q, CancellationToken ct)
        {
            var ev = await _eventRepo.GetByIdWithParticipantsAsync(q.EventId, ct);
            if (ev is null) return Result<EventDetailDto>.Failure("Event not found.");

            bool isJoined = q.RequestingUserId.HasValue &&
                ev.Participants.Any(p =>
                    p.UserId == q.RequestingUserId.Value &&
                    p.Status == Domain.Enums.ParticipantStatus.Joined);

            var dto = new EventDetailDto
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                SportType = ev.SportType.ToString(),
                DifficultyLevel = ev.DifficultyLevel.ToString(),
                Status = ev.Status.ToString(),
                LocationName = ev.LocationName,
                Latitude = ev.Latitude,
                Longitude = ev.Longitude,
                EventDate = ev.EventDate,
                CreatedAt = ev.CreatedAt,
                ParticipantCount = ev.ParticipantCount,
                MaxParticipants = ev.MaxParticipants,
                IsFull = ev.IsFull,
                DistanceKm = ev.DistanceKm,
                CoverImageUrl = ev.CoverImageUrl,
                RouteImageUrl = ev.RouteImageUrl,
                Organizer = new OrganizerSummaryDto
                {
                    Id = ev.OrganizerId,
                    FullName = $"{ev.Organizer.FirstName} {ev.Organizer.LastName}".Trim(),
                    AvatarUrl = null
                },
                IsJoined = isJoined,
                Participants = ev.Participants
                    .Where(p => p.Status == Domain.Enums.ParticipantStatus.Joined)
                    .Select(p => new ParticipantDto
                    {
                        UserId = p.UserId,
                        FullName = $"{p.User.FirstName} {p.User.LastName}".Trim(),
                        AvatarUrl = null,
                        JoinedAt = p.JoinedAt,
                        Status = p.Status.ToString()
                    })
            };

            return Result<EventDetailDto>.Success(dto);
        }
    }
}