using AZM.Application.Auth.DTOs.Event;
using AZM.Application.Auth.DTOs.Participants;
using AZM.Application.Events.Queries;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class GetEventByIdHandler : IRequestHandler<GetEventByIdQuery, EventDto?>
    {
        private readonly IEventRepository _eventRepo;

        public GetEventByIdHandler(IEventRepository eventRepo)
        {
            _eventRepo = eventRepo;
        }

        public async Task<EventDto?> Handle(GetEventByIdQuery query, CancellationToken ct)
        {
            var ev = await _eventRepo.GetByIdAsync(query.EventId);
            if (ev is null) return null;

            return new EventDto
            {
                Id = ev.Id,
                Title = ev.Title,
                Description = ev.Description,
                Difficulty = ev.Difficulty,
                Status = ev.Status,
                StartAtUtc = ev.StartAtUtc,
                MeetingLat = ev.MeetingLat,
                MeetingLng = ev.MeetingLng,
                MeetingAddress = ev.MeetingAddress,
                MaxParticipants = ev.MaxParticipants,
                SportType = ev.SportType,
                CreatedByUserId = ev.CreatedByUserId,
                CreatedByName = ev.CreatedByUser is not null
                    ? $"{ev.CreatedByUser.FirstName} {ev.CreatedByUser.LastName}"
                    : string.Empty,
                IsFull = ev.IsFull,
                ParticipantCount = ev.Participants.Count,
                Participants = ev.Participants.Select(p => new ParticipantDto
                {
                    UserId = p.UserId,
                    FullName = p.User is not null ? $"{p.User.FirstName} {p.User.LastName}" : string.Empty,
                    Email = p.User?.Email ?? string.Empty,
                    Status = p.Status,
                    JoinedAtUtc = p.JoinedAtUtc
                }).ToList()
            };
        }
    }
}