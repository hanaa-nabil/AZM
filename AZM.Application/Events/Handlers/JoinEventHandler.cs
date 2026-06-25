using AZM.Application.Common;
using AZM.Application.Events.Commands;
using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class JoinEventHandler : IRequestHandler<JoinEventCommand, Result<bool>>
    {
        private readonly IEventRepository _eventRepo;

        public JoinEventHandler(IEventRepository eventRepo)
        {
            _eventRepo = eventRepo;
        }

        public async Task<Result<bool>> Handle(JoinEventCommand command, CancellationToken ct)
        {
            var ev = await _eventRepo.GetByIdAsync(command.EventId);
            if (ev is null)
                return Result<bool>.Failure("Event not found");

            if (ev.Status == EventStatus.Cancelled || ev.Status == EventStatus.Completed)
                return Result<bool>.Failure("Cannot join this event");

            if (ev.IsFull)
                return Result<bool>.Failure("Event is full");

            var alreadyJoined = ev.Participants
                .Any(p => p.UserId == command.UserId && p.Status == ParticipantStatus.Joined);

            if (alreadyJoined)
                return Result<bool>.Failure("Already joined");

            ev.Participants.Add(new EventParticipant
            {
                EventId = command.EventId,
                UserId = command.UserId,
                Status = ParticipantStatus.Joined,
                JoinedAtUtc = DateTime.UtcNow
            });

            await _eventRepo.UpdateAsync(ev, ct);
            return Result<bool>.Success(true);
        }
    }
}