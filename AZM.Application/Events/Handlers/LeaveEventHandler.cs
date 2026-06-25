using AZM.Application.Common;
using AZM.Application.Events.Commands;
using AZM.Application.Events.Queries;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class LeaveEventHandler : IRequestHandler<LeaveEventCommand, Result<bool>>
    {
        private readonly IEventRepository _eventRepo;

        public LeaveEventHandler(IEventRepository eventRepo)
        {
            _eventRepo = eventRepo;
        }

        public async Task<Result<bool>> Handle(LeaveEventCommand command, CancellationToken ct)
        {
            var ev = await _eventRepo.GetByIdAsync(command.EventId);
            if (ev is null)
                return Result<bool>.Failure("Event not found");

            var participant = ev.Participants
                .FirstOrDefault(p => p.UserId == command.UserId && p.Status == ParticipantStatus.Joined);

            if (participant is null)
                return Result<bool>.Failure("You are not a participant of this event");

            participant.Status = ParticipantStatus.Left;

            await _eventRepo.UpdateAsync(ev, ct);
            return Result<bool>.Success(true);
        }
    }
}