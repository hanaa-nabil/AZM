using AZM.Application.Common;
using AZM.Application.DTOs.Squad;
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
    public class SendSquadMessageHandler : IRequestHandler<SendSquadMessageCommand, Result<SquadMessageDto>>
    {
        private readonly ISquadRepository _squads;
        private readonly ISquadMessageRepository _messages;
        private readonly IUserRepository _users;
        private readonly ISquadPresence _presence;
        private readonly INotificationService _notifications;
        private readonly ILogger<SendSquadMessageHandler> _logger;

        public SendSquadMessageHandler(ISquadRepository squads, ISquadMessageRepository messages,
            IUserRepository users, ISquadPresence presence, INotificationService notifications,
            ILogger<SendSquadMessageHandler> logger)
        {
            _squads = squads; _messages = messages; _users = users;
            _presence = presence; _notifications = notifications; _logger = logger;
        }

        public async Task<Result<SquadMessageDto>> Handle(SendSquadMessageCommand cmd, CancellationToken ct)
        {
            var content = cmd.Content?.Trim();
            if (string.IsNullOrEmpty(content) || content.Length > 2000)
                return Result<SquadMessageDto>.Failure("Message must be 1-2000 characters.", 400);

            var member = await _squads.GetMemberAsync(cmd.SquadId, cmd.SenderId, ct);
            if (member is null || member.Status != SquadMemberStatus.Approved)
                return Result<SquadMessageDto>.Failure("Only squad members can send messages.", 403);

            var message = SquadMessage.Create(cmd.SquadId, cmd.SenderId, content);
            await _messages.AddAsync(message, ct);

            // Sending counts as reading your own chat
            member.MarkRead();
            await _squads.UpdateMemberAsync(member, ct);

            var sender = await _users.GetByIdAsync(cmd.SenderId.ToString());

            // Push only to members who are not currently in the chat room
            try
            {
                var squad = await _squads.GetByIdWithMembersAsync(cmd.SquadId, ct);
                var offline = squad!.Members
                    .Where(m => m.Status == SquadMemberStatus.Approved && m.UserId != cmd.SenderId
                                && !_presence.IsOnline(cmd.SquadId, m.UserId))
                    .Select(m => m.UserId).ToList();

                if (offline.Count > 0)
                    await _notifications.SendBulkAsync(offline, NotificationType.SquadMessage,
                        squad.Name, content, actorId: cmd.SenderId, squadId: cmd.SquadId, ct: ct);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Squad message saved but push failed for squad {SquadId}.", cmd.SquadId);
            }

            return Result<SquadMessageDto>.Success(new SquadMessageDto
            {
                Id = message.Id,
                SquadId = message.SquadId,
                SenderId = message.SenderId,
                SenderName = sender?.FullName ?? string.Empty,
                SenderAvatar = sender?.ProfilePhotoUrl,
                Content = message.Content,
                SentAt = message.SentAt,
                Seen = false
            });
        }
    }
}
