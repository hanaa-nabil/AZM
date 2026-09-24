using AZM.Application.Common;
using AZM.Application.DTOs.Event;
using AZM.Application.DTOs.Participants;
using AZM.Application.Events.Queries;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class GetEventParticipantsHandler : IRequestHandler<GetEventParticipantsQuery, Result<ParticipantListDto>>
    {
        private readonly IEventRepository _eventRepo;

        public GetEventParticipantsHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<ParticipantListDto>> Handle(GetEventParticipantsQuery q, CancellationToken ct)
        {
            var ev = await _eventRepo.GetByIdWithParticipantsAsync(q.EventId, ct);
            if (ev is null) return Result<ParticipantListDto>.Failure("Event not found.");

            var joined = ev.Participants
                .Where(p => p.Status == Domain.Enums.ParticipantStatus.Joined)
                .Select(p => new ParticipantDto
                {
                    Id = p.UserId,
                    FullName = $"{p.User.FirstName} {p.User.LastName}".Trim(),
                    Username = p.User.UserName ?? string.Empty,
                    AvatarUrl = p.User.ProfilePhotoUrl,
                    JoinedAt = p.JoinedAt,
                    Status = p.Status.ToString()
                });

            return Result<ParticipantListDto>.Success(new ParticipantListDto
            {
                EventId = ev.Id,
                EventTitle = ev.Title,
                TotalJoined = ev.ParticipantCount,
                Organizer = new OrganizerSummaryDto
                {
                    Id = ev.OrganizerId,
                    FullName = $"{ev.Organizer.FirstName} {ev.Organizer.LastName}".Trim(),
                    Username = ev.Organizer.UserName ?? string.Empty,
                    AvatarUrl = ev.Organizer.ProfilePhotoUrl,
                    IsVerified = ev.Organizer.IsIdVerified && ev.Organizer.IsFaceVerified
                },
                Participants = joined
            });
        }
    }
}