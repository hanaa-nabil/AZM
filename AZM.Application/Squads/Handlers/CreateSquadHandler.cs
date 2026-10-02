using AZM.Application.Common;
using AZM.Application.Squad.Commands;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Squad.Handlers
{
    public class CreateSquadHandler : IRequestHandler<CreateSquadCommand, Result<Guid>>
    {
        private readonly ISquadRepository _squadRepo;

        public CreateSquadHandler(ISquadRepository squadRepo) => _squadRepo = squadRepo;

        public async Task<Result<Guid>> Handle(CreateSquadCommand cmd, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cmd.Name))
                return Result<Guid>.Failure("Squad name is required.");

            var squad = Domain.Entities.Squad.Create(cmd.Name.Trim(), cmd.Bio, cmd.Sports,
                cmd.Latitude, cmd.Longitude, cmd.LocationName, cmd.Privacy,
                cmd.CoverImageUrl, cmd.FounderId);

            await _squadRepo.AddAsync(squad, ct);

            var founderMembership = SquadMember.CreateFounder(squad.Id, cmd.FounderId);
            await _squadRepo.AddMemberAsync(founderMembership, ct);

            return Result<Guid>.Success(squad.Id);
        }
    }
}
