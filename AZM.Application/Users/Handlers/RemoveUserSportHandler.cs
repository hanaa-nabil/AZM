using AZM.Application.Users.Commands;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Handlers
{
    public class RemoveUserSportHandler : IRequestHandler<RemoveUserSportCommand, Unit>
    {
        private readonly IUserRepository _userRepository;

        public RemoveUserSportHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(RemoveUserSportCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(request.UserId)
                ?? throw new KeyNotFoundException("User not found.");

            var sport = user.Sports.FirstOrDefault(s => s.Sport == request.Sport);
            if (sport is null) return Unit.Value; 

            user.Sports.Remove(sport);
            await _userRepository.UpdateAsync(user);
            return Unit.Value;
        }
    }

}