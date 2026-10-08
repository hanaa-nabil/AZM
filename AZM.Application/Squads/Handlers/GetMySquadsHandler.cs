using AZM.Application.DTOs.Squad;
using AZM.Application.Squads.Queries;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Squads.Handlers
{
    public class GetMySquadsHandler : IRequestHandler<GetMySquadsQuery, List<SquadSummaryDto>>
    {
        private readonly ISquadRepository _squadRepo;
        public GetMySquadsHandler(ISquadRepository squadRepo) => _squadRepo = squadRepo;

        public async Task<List<SquadSummaryDto>> Handle(GetMySquadsQuery q, CancellationToken ct)
        {
            var squads = await _squadRepo.GetMySquadsAsync(q.UserId, ct);
            return squads.Select(s => new SquadSummaryDto
            {
                Id = s.Id,
                Name = s.Name,
                CoverImageUrl = s.CoverImageUrl,
                MemberCount = s.MemberCount,
                Privacy = s.Privacy.ToString(),
                Sports = s.Sports.Select(sp => sp.ToString()).ToList(),
                LocationName = s.LocationName
            }).ToList();
        }
    }
}
