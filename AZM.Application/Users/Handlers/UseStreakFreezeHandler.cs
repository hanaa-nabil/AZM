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
    public class UseStreakFreezeHandler : IRequestHandler<UseStreakFreezeCommand, bool>
    {
        private readonly IUserRepository _userRepository;

        public UseStreakFreezeHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(UseStreakFreezeCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(request.UserId)
                ?? throw new KeyNotFoundException("User not found.");

            var profile = user.Profile
                ?? throw new InvalidOperationException("User has no profile.");

            if (profile.StreakFreezesAvailable <= 0)
                return false;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            profile.RegisterActivity(today);

            await _userRepository.UpdateAsync(user);
            return true;
        }
    }
}
