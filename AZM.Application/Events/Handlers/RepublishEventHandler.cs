using AZM.Application.Common;
using AZM.Application.Events.Commands;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Handlers
{
    public class RepublishEventHandler : IRequestHandler<RepublishEventCommand, Result<bool>>
    {
        private readonly IEventRepository _eventRepo;

        public RepublishEventHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<bool>> Handle(RepublishEventCommand cmd, CancellationToken ct)
        {
            var ev = await _eventRepo.GetByIdAsync(cmd.EventId, ct);
            if (ev is null) return Result<bool>.Failure("Event not found.");

            if (ev.OrganizerId != cmd.RequestingUserId)
                return Result<bool>.Failure("Only the organizer can republish this event.", 403);

            if (ev.Status != EventStatus.Cancelled)
                return Result<bool>.Failure("Only cancelled events can be republished.");

            if (ev.EventDate <= DateTime.UtcNow)
                return Result<bool>.Failure("Cannot republish an event whose date has already passed. Update the date first.");

            ev.Publish ();
            await _eventRepo.UpdateAsync(ev, ct);

            return Result<bool>.Success(true);
        }
    }
}
