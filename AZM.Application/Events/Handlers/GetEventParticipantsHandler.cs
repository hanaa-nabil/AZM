using AZM.Application.Auth.DTOs.Participants;
using AZM.Application.Common;
using AZM.Application.Events.Queries;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Handlers
{
    public class GetEventParticipantsHandler : IRequestHandler<GetEventParticipantsQuery, Result<ParticipantListDto>>
    {
        private readonly IEventRepository _eventRepo;

        public GetEventParticipantsHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<ParticipantListDto>> Handle(GetEventParticipantsQuery q, CancellationToken ct)
        {
            var ev = await _eventRepo.GetByIdWithParticipantsAsync(q.EventId, ct);
            if (ev is null) return Result<ParticipantListDto>.Failure("Event not found.");

            var joined = ev.Participants
                .Where(p => p.Status == Domain.Enums.ParticipantStatus.Joined)
                .Select(p => new ParticipantDto
                {
                    UserId = p.UserId,
                    FullName = $"{p.User.FirstName} {p.User.LastName}".Trim(),
                    AvatarUrl = null,
                    JoinedAt = p.JoinedAt,
                    Status = p.Status.ToString()
                });

            return Result<ParticipantListDto>.Success(new ParticipantListDto
            {
                EventId = ev.Id,
                EventTitle = ev.Title,
                TotalJoined = ev.ParticipantCount,
                Participants = joined
            });
        }
    }

}
