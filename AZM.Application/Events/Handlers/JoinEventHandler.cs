using AZM.Application.Common;
using AZM.Application.Events.Commands;
using AZM.Domain.DomainEvents;
using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class JoinEventHandler : IRequestHandler<JoinEventCommand, Result<bool>>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;   

        public JoinEventHandler(
            IEventRepository eventRepository,
            IUserRepository userRepository,
            IMediator mediator)                
        {
            _eventRepo = eventRepository;
            _userRepository = userRepository;
            _mediator = mediator;              
        }

        public async Task<Result<bool>> Handle(JoinEventCommand cmd, CancellationToken ct)
        {
            var ev = await _eventRepo.GetByIdAsync(cmd.EventId, ct);
            if (ev is null) return Result<bool>.Failure("Event not found.");
            if (ev.Status == EventStatus.Cancelled)
                return Result<bool>.Failure("Cannot join a cancelled event.");
            if (ev.Status == EventStatus.Completed)
                return Result<bool>.Failure("Cannot join a completed event.");
            if (ev.OrganizerId == cmd.UserId)
                return Result<bool>.Failure("Organizer cannot join their own event.");

            var existing = await _eventRepo.GetParticipantAsync(cmd.EventId, cmd.UserId, ct);
            if (existing is not null)
            {
                if (existing.Status == ParticipantStatus.Joined)
                    return Result<bool>.Failure("You have already joined this event.");
                existing.Rejoin();
                await _eventRepo.UpdateParticipantAsync(existing, ct);
            }
            else
            {
                if (ev.MaxParticipants > 0)
                {
                    var count = await _eventRepo.GetParticipantCountAsync(cmd.EventId, ct);
                    if (count >= ev.MaxParticipants)
                        return Result<bool>.Failure("This event is full.");
                }

                var participant = EventParticipant.Create(cmd.EventId, cmd.UserId);
                await _eventRepo.AddParticipantAsync(participant, ct);
            }

            var user = await _userRepository.GetByIdWithDetailsAsync(cmd.UserId);
            if (user?.Profile is not null)
            {
                user.Profile.EventsJoinedCount++;
                await _userRepository.UpdateAsync(user);
            }

            await _mediator.Publish(new EventParticipantJoined(ev.Id, ev.OrganizerId, cmd.UserId), ct);

            return Result<bool>.Success(true);
        }
    }
}