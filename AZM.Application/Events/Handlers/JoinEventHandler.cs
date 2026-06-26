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

        public JoinEventHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<bool>> Handle(JoinEventCommand cmd, CancellationToken ct)
        {
            var ev = await _eventRepo.GetByIdAsync(cmd.EventId, ct);
            if (ev is null) return Result<bool>.Failure("Event not found.");
            if (ev.Status == Domain.Enums.EventStatus.Cancelled)
                return Result<bool>.Failure("Cannot join a cancelled event.");
            if (ev.Status == Domain.Enums.EventStatus.Completed)
                return Result<bool>.Failure("Cannot join a completed event.");
            if (ev.OrganizerId == cmd.UserId)
                return Result<bool>.Failure("Organizer cannot join their own event.");

            var existing = await _eventRepo.GetParticipantAsync(cmd.EventId, cmd.UserId, ct);
            if (existing is not null)
            {
                if (existing.Status == Domain.Enums.ParticipantStatus.Joined)
                    return Result<bool>.Failure("You have already joined this event.");
                existing.Rejoin();
                await _eventRepo.UpdateParticipantAsync(existing, ct);
            }
            else
            {
                // Check capacity
                if (ev.MaxParticipants > 0)
                {
                    var count = await _eventRepo.GetParticipantCountAsync(cmd.EventId, ct);
                    if (count >= ev.MaxParticipants)
                        return Result<bool>.Failure("This event is full.");
                }

                var participant = EventParticipant.Create(cmd.EventId, cmd.UserId);
                await _eventRepo.AddParticipantAsync(participant, ct);
            }

            return Result<bool>.Success(true);
        }
    }
}