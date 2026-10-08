using AZM.Application.Common;
using AZM.Application.DTOs.Squad;
using AZM.Application.Squads.Queries;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Squads.Handlers
{
    public class GetSquadMessagesHandler : IRequestHandler<GetSquadMessagesQuery, Result<SquadChatDto>>
    {
        private readonly ISquadRepository _squads;
        private readonly ISquadMessageRepository _messages;
        private readonly IEventRepository _events;
        private readonly ISquadPresence _presence;

        public GetSquadMessagesHandler(ISquadRepository squads, ISquadMessageRepository messages,
            IEventRepository events, ISquadPresence presence)
        {
            _squads = squads; _messages = messages; _events = events; _presence = presence;
        }

        public async Task<Result<SquadChatDto>> Handle(GetSquadMessagesQuery q, CancellationToken ct)
        {
            var member = await _squads.GetMemberAsync(q.SquadId, q.UserId, ct);
            if (member is null || member.Status != SquadMemberStatus.Approved)
                return Result<SquadChatDto>.Failure("Only squad members can view the chat.", 403);

            var squad = await _squads.GetByIdWithMembersAsync(q.SquadId, ct);
            if (squad is null) return Result<SquadChatDto>.Failure("Squad not found.", 404);

            var page = await _messages.GetPageAsync(q.SquadId, q.Page, q.PageSize, ct);

            var reads = squad.Members
                .Where(m => m.Status == SquadMemberStatus.Approved && m.LastReadAt.HasValue)
                .Select(m => (m.UserId, At: m.LastReadAt!.Value)).ToList();

            PinnedEventDto? pinned = null;
            if (squad.PinnedEventId.HasValue)
            {
                var ev = await _events.GetByIdAsync(squad.PinnedEventId.Value, ct);
                if (ev is not null && ev.Status != EventStatus.Cancelled)
                    pinned = new PinnedEventDto { Id = ev.Id, Title = ev.Title, EventDate = ev.EventDate };
            }

            return Result<SquadChatDto>.Success(new SquadChatDto
            {
                SquadName = squad.Name,
                CoverImageUrl = squad.CoverImageUrl,
                OnlineCount = _presence.OnlineCount(q.SquadId),
                PinnedEvent = pinned,
                Messages = page.OrderBy(m => m.SentAt).Select(m => new SquadMessageDto
                {
                    Id = m.Id,
                    SquadId = m.SquadId,
                    SenderId = m.SenderId,
                    SenderName = m.Sender.FullName,
                    SenderAvatar = m.Sender.ProfilePhotoUrl,
                    Content = m.Content,
                    SentAt = m.SentAt,
                    IsDeleted = m.IsDeleted,
                    Seen = reads.Any(r => r.UserId != m.SenderId && r.At >= m.SentAt)
                }).ToList()
            });
        }
    }
}
