using AZM.Application.Squads.Commands;
using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Hubs
{
    [Authorize]
    public class SquadChatHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly ISquadRepository _squads;
        private readonly ISquadPresence _presence;

        public SquadChatHub(IMediator mediator, ISquadRepository squads, ISquadPresence presence)
        { _mediator = mediator; _squads = squads; _presence = presence; }

        private Guid UserId =>
            Guid.Parse(Context.User!.FindFirst(JwtRegisteredClaimNames.Sub)!.Value);

        public async Task JoinSquadRoom(Guid squadId)
        {
            var member = await _squads.GetMemberAsync(squadId, UserId);
            if (member is null || member.Status != SquadMemberStatus.Approved)
                throw new HubException("Not a member of this squad.");

            await Groups.AddToGroupAsync(Context.ConnectionId, squadId.ToString());
            _presence.Connect(squadId, UserId, Context.ConnectionId);
            await Clients.Group(squadId.ToString()).SendAsync("OnlineCountChanged", _presence.OnlineCount(squadId));
        }

        public async Task LeaveSquadRoom(Guid squadId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, squadId.ToString());
            _presence.Disconnect(Context.ConnectionId);
            await Clients.Group(squadId.ToString()).SendAsync("OnlineCountChanged", _presence.OnlineCount(squadId));
        }

        public async Task SendMessage(Guid squadId, string content)
        {
            var result = await _mediator.Send(new SendSquadMessageCommand(squadId, UserId, content));
            if (!result.IsSuccess) throw new HubException(result.Error);

            await Clients.Group(squadId.ToString()).SendAsync("ReceiveMessage", result.Data);
        }

        // Client calls this when the chat screen is open or new messages are visible
        public async Task MarkRead(Guid squadId)
        {
            var result = await _mediator.Send(new MarkSquadReadCommand(squadId, UserId));
            if (!result.IsSuccess) throw new HubException(result.Error);

            await Clients.OthersInGroup(squadId.ToString())
                .SendAsync("MessagesRead", new { userId = UserId, readAt = DateTime.UtcNow });
        }

        public async Task DeleteMessage(Guid messageId)
        {
            var result = await _mediator.Send(new DeleteSquadMessageCommand(messageId, UserId));
            if (!result.IsSuccess) throw new HubException(result.Error);

            await Clients.Group(result.Data.ToString()).SendAsync("MessageDeleted", messageId);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _presence.Disconnect(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
