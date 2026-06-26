using AZM.Application.Common;
using AZM.Application.Events.Commands;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Handlers
{
    public class CancelEventHandler : IRequestHandler<CancelEventCommand, Result<bool>>
    {
        private readonly IEventRepository _eventRepo;

        public CancelEventHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<bool>> Handle(CancelEventCommand cmd, CancellationToken ct)
        {
            var ev = await _eventRepo.GetByIdAsync(cmd.EventId, ct);
            if (ev is null) return Result<bool>.Failure("Event not found.");
            if (ev.OrganizerId != cmd.RequestingUserId)
                return Result<bool>.Failure("Only the organizer can cancel this event.");

            ev.Cancel();
            await _eventRepo.UpdateAsync(ev, ct);
            return Result<bool>.Success(true);
        }
    }
}
