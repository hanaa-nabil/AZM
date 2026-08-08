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
    public class UpdateFcmTokenCommandHandler : IRequestHandler<UpdateFcmTokenCommand, Result>
    {
        private readonly IUserRepository _userRepository;

        public UpdateFcmTokenCommandHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result> Handle(UpdateFcmTokenCommand request, CancellationToken cancellationToken)
        {
            await _userRepository.UpdateFcmTokenAsync(request.UserId, request.FcmToken);
            return Result.Success();
        }
    }
}
