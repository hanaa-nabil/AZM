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

        public LeaveEventHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<bool>> Handle(LeaveEventCommand cmd, CancellationToken ct)
        {
            var participant = await _eventRepo.GetParticipantAsync(cmd.EventId, cmd.UserId, ct);
            if (participant is null || participant.Status != Domain.Enums.ParticipantStatus.Joined)
                return Result<bool>.Failure("You are not a participant of this event.");

            participant.Leave();
            await _eventRepo.UpdateParticipantAsync(participant, ct);
            return Result<bool>.Success(true);
        }
    }
}