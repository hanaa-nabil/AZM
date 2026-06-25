using AZM.Application.Auth.DTOs.Event;
using AZM.Application.Common;
using AZM.Application.Events.Commands;
using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class CreateEventHandler : IRequestHandler<CreateEventCommand, Result<EventDto>>
    {
        private readonly IEventRepository _eventRepo;

        public CreateEventHandler(IEventRepository eventRepo)
        {
            _eventRepo = eventRepo;
        }

        public async Task<Result<EventDto>> Handle(CreateEventCommand command, CancellationToken ct)
        {
            var ev = new Event
            {
                Title = command.Title,
                Description = command.Description,
                Difficulty = command.Difficulty,
                StartAtUtc = command.StartAtUtc,
                MaxParticipants = command.MaxParticipants,
                MeetingLat = command.MeetingLat,
                MeetingLng = command.MeetingLng,
                MeetingAddress = command.MeetingAddress,
                SportType = command.SportType,
                CreatedByUserId = command.CreatedByUserId,
                Status = EventStatus.Scheduled  // no publish step needed
            };

            await _eventRepo.AddAsync(ev, ct);

            return Result<EventDto>.Success(new EventDto
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
                ParticipantCount = 0,
                Participants = new()
            });
        }
    }
}