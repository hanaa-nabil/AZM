using AZM.Application.Users.Commands;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Users.Handlers
{
    public class AddUserSportHandler : IRequestHandler<AddUserSportCommand, Unit>
    {
        private readonly IUserRepository _userRepository;

        public AddUserSportHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(AddUserSportCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(request.UserId)
                  ?? throw new KeyNotFoundException("User not found.");

            if (user.Sports.Any(s => s.Sport == request.Sport))
                return Unit.Value; 

            user.Sports.Add(new UserSport
            {
                UserId = user.Id,
                Sport = request.Sport
            });

            await _userRepository.UpdateAsync(user);
            return Unit.Value;
        }
    }
}
