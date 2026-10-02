using AZM.Application.Common;
using AZM.Application.Squads.Commands;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Squads.Handlers
{
    public class DeleteSquadHandler : IRequestHandler<DeleteSquadCommand, Result<bool>>
    {
        private readonly ISquadRepository _squadRepo;

        public DeleteSquadHandler(ISquadRepository squadRepo) => _squadRepo = squadRepo;

        public async Task<Result<bool>> Handle(DeleteSquadCommand cmd, CancellationToken ct)
        {
            var squad = await _squadRepo.GetByIdAsync(cmd.SquadId, ct);
            if (squad is null) return Result<bool>.Failure("Squad not found.", 404);

            if (squad.FounderId != cmd.RequestingUserId)
                return Result<bool>.Failure("Only the founder can delete this squad.", 403);

            await _squadRepo.DeleteAsync(cmd.SquadId, ct);
            return Result<bool>.Success(true);
        }
    }
}
