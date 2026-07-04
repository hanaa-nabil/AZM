using AZM.Application.Common;
using AZM.Application.Events.Commands;
using AZM.Domain.DomainEvents;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Handlers
{
    public class PublishEventCommandHandler : IRequestHandler<PublishEventCommand, Result<bool>>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IMediator _mediator;

        public PublishEventCommandHandler(IEventRepository eventRepo, IMediator mediator)
        {
            _eventRepo = eventRepo;
            _mediator = mediator;
        }

        public async Task<Result<bool>> Handle(PublishEventCommand request, CancellationToken cancellationToken)
        {
            var ev = await _eventRepo.GetByIdAsync(request.EventId, cancellationToken);
            if (ev is null)
                return Result<bool>.Failure("Event not found");

            if (ev.OrganizerId != request.RequestingUserId)
                return Result<bool>.Failure("Only the organizer can publish this event");

            ev.Publish();
            await _eventRepo.UpdateAsync(ev, cancellationToken);

            await _mediator.Publish(new EventPublished(ev.Id, ev.OrganizerId), cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
