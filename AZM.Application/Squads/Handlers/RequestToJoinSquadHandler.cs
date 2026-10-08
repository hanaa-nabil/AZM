using AZM.Application.Common;
using AZM.Application.Squad.Commands;
using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

namespace AZM.Application.Squad.Handlers
{
    public class RequestToJoinSquadHandler : IRequestHandler<RequestToJoinSquadCommand, Result<string>>
    {
        private readonly ISquadRepository _squadRepo;
        private readonly IUserRepository _users;
        private readonly INotificationService _notifications;
        private readonly ILogger<RequestToJoinSquadHandler> _logger;

        public RequestToJoinSquadHandler(
            ISquadRepository squadRepo,
            IUserRepository users,
            INotificationService notifications,
            ILogger<RequestToJoinSquadHandler> logger)
        {
            _squadRepo = squadRepo;
            _users = users;
            _notifications = notifications;
            _logger = logger;
        }

        public async Task<Result<string>> Handle(RequestToJoinSquadCommand cmd, CancellationToken ct)
        {
            var squad = await _squadRepo.GetByIdAsync(cmd.SquadId, ct);
            if (squad is null) return Result<string>.Failure("Squad not found.", 404);

            var existing = await _squadRepo.GetMemberAsync(cmd.SquadId, cmd.UserId, ct);
            if (existing is not null && existing.Status != SquadMemberStatus.Removed)
                return Result<string>.Failure(
                    existing.Status == SquadMemberStatus.Pending
                        ? "Your request is already pending."
                        : "You're already a member of this squad.", 409);

            var autoApprove = squad.Privacy == SquadPrivacy.Public;

            if (existing is not null)
            {
                // Previously removed: reuse the row instead of inserting a duplicate
                existing.Reapply(autoApprove);
                await _squadRepo.UpdateMemberAsync(existing, ct);
            }
            else
            {
                var member = SquadMember.RequestToJoin(cmd.SquadId, cmd.UserId, autoApprove);
                await _squadRepo.AddMemberAsync(member, ct);
            }

            try
            {
                var user = await _users.GetByIdAsync(cmd.UserId.ToString());
                var name = user?.FullName ?? "Someone";

                var full = await _squadRepo.GetByIdWithMembersAsync(cmd.SquadId, ct);
                var staff = full!.Members
                    .Where(m => m.Status == SquadMemberStatus.Approved && m.Role != SquadMemberRole.Member)
                    .Select(m => m.UserId)
                    .ToList();

                if (staff.Count > 0)
                    await _notifications.SendBulkAsync(
                        staff,
                        autoApprove ? NotificationType.SquadMemberJoined : NotificationType.SquadJoinRequest,
                        squad.Name,
                        autoApprove ? $"{name} joined your squad." : $"{name} wants to join your squad.",
                        actorId: cmd.UserId,
                        squadId: cmd.SquadId,
                        ct: ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Squad join notification failed for squad {SquadId}.", cmd.SquadId);
            }

            return Result<string>.Success(autoApprove ? "Joined" : "Pending");
        }
       
    }
}