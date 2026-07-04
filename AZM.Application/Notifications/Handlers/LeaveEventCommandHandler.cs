using AZM.Application.Common;
using AZM.Application.Events.Commands;
using AZM.Domain.DomainEvents;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Notifications.Handlers
{
    public class LeaveEventCommandHandler : IRequestHandler<LeaveEventCommand, Result<bool>>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IMediator _mediator;

        public LeaveEventCommandHandler(IEventRepository eventRepo, IMediator mediator)
        {
            _eventRepo = eventRepo;
            _mediator = mediator;
        }

        public async Task<Result<bool>> Handle(LeaveEventCommand request, CancellationToken cancellationToken)
        {
            var ev = await _eventRepo.GetByIdAsync(request.EventId, cancellationToken);
            if (ev is null)
                return Result<bool>.Failure("Event not found");

            if (ev.OrganizerId == request.UserId)
                return Result<bool>.Failure("Organizer cannot leave their own event");

            var participant = await _eventRepo.GetParticipantAsync(request.EventId, request.UserId, cancellationToken);

            if (participant is null || participant.Status != ParticipantStatus.Joined)
                return Result<bool>.Failure("You are not a participant of this event");

            participant.Leave();

            await _eventRepo.UpdateParticipantAsync(participant, cancellationToken);

            await _mediator.Publish(
                new EventParticipantLeft(ev.Id, ev.OrganizerId, request.UserId),
                cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
