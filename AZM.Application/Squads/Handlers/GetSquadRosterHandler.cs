using AZM.Application.Common;
using AZM.Application.DTOs.Squad;
using AZM.Application.Squads.Queries;
using AZM.Domain.Entities;
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
    public class GetSquadRosterHandler : IRequestHandler<GetSquadRosterQuery, Result<SquadDetailDto>>
    {
        private readonly ISquadRepository _squadRepo;

        public GetSquadRosterHandler(ISquadRepository squadRepo) => _squadRepo = squadRepo;

        public async Task<Result<SquadDetailDto>> Handle(GetSquadRosterQuery q, CancellationToken ct)
        {
            var squad = await _squadRepo.GetByIdWithMembersAsync(q.SquadId, ct);
            if (squad is null) return Result<SquadDetailDto>.Failure("Squad not found.", 404);

            var roster = squad.Members.Where(m => m.Status == SquadMemberStatus.Approved)
                .Select(ToDto).ToList();
            var pending = squad.Members.Where(m => m.Status == SquadMemberStatus.Pending)
                .Select(ToDto).ToList();

            return Result<SquadDetailDto>.Success(new SquadDetailDto
            {
                Id = squad.Id,
                Name = squad.Name,
                Bio = squad.Bio,
                Privacy = squad.Privacy.ToString(),
                MemberCount = roster.Count,
                Roster = roster,
                PendingRequests = pending
            });
        }

        private static SquadMemberDto ToDto(SquadMember m) => new()
        {
            UserId = m.UserId,
            FullName = $"{m.User.FirstName} {m.User.LastName}".Trim(),
            Username = m.User.UserName ?? string.Empty,
            AvatarUrl = m.User.ProfilePhotoUrl,
            Role = m.Role.ToString(),
            ApprovedAt = m.ApprovedAt
        };
    }
}
