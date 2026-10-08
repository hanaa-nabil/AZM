using AZM.Application.Common;
using AZM.Application.Squads.Commands;
using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Squads.Handlers
{

    public class PinSquadEventHandler : IRequestHandler<PinSquadEventCommand, Result<bool>>
    {
        private readonly ISquadRepository _squads;
        private readonly IEventRepository _events;
        private readonly INotificationService _notifications;
        private readonly ILogger<PinSquadEventHandler> _logger;

        public PinSquadEventHandler(ISquadRepository squads, IEventRepository events, INotificationService notifications, ILogger<PinSquadEventHandler> logger)
        {
            _squads = squads;
            _events = events; 
            _notifications = notifications; 
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(PinSquadEventCommand cmd, CancellationToken ct)
        {
            var member = await _squads.GetMemberAsync(cmd.SquadId, cmd.RequestingUserId, ct);
            if (member is null || member.Status != SquadMemberStatus.Approved || member.Role == SquadMemberRole.Member)
                return Result<bool>.Failure("Only the founder or a co-captain can pin an event.", 403);

            if (cmd.EventId.HasValue && !await _events.ExistsAsync(cmd.EventId.Value, ct))
                return Result<bool>.Failure("Event not found.", 404);

            var squad = await _squads.GetByIdAsync(cmd.SquadId, ct);
            if (squad is null) return Result<bool>.Failure("Squad not found.", 404);

            squad.PinEvent(cmd.EventId);
            await _squads.UpdateAsync(squad, ct);

            if (cmd.EventId.HasValue)
            {
                try
                {
                    var ev = await _events.GetByIdAsync(cmd.EventId.Value, ct);
                    var full = await _squads.GetByIdWithMembersAsync(cmd.SquadId, ct);
                    var recipients = full!.Members
                        .Where(m => m.Status == SquadMemberStatus.Approved && m.UserId != cmd.RequestingUserId)
                        .Select(m => m.UserId).ToList();

                    if (recipients.Count > 0)
                        await _notifications.SendBulkAsync(
                            recipients,
                            NotificationType.SquadEventPinned,
                            squad.Name,
                            $"Pinned event: {ev?.Title}",
                            relatedEventId: cmd.EventId,
                            actorId: cmd.RequestingUserId,
                            squadId: cmd.SquadId,
                            ct: ct);
                }
                catch (Exception ex) { _logger.LogError(ex, "Pin notification failed."); }
            }
            return Result<bool>.Success(true);
        }
    }
}
