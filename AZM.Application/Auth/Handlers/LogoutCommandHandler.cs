using AZM.Application.Auth.Commands;
using AZM.Application.Common;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Auth.Handlers
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IUserRepository _userRepository;

        public LogoutCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            // Clears the stored FCM token so this device stops receiving push
            // notifications after logout. The JWT itself isn't revoked (stateless) —
            // the client is responsible for discarding it locally.
            await _userRepository.UpdateFcmTokenAsync(request.UserId, string.Empty);

            return Result.Success();
        }
    }
}
