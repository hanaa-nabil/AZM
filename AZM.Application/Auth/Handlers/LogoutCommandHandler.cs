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
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LogoutCommandHandler(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(request.Dto.FcmToken))
                await _userRepository.ClearFcmTokenAsync(request.UserId, request.Dto.FcmToken);

            if (!string.IsNullOrWhiteSpace(request.Dto.RefreshToken))
                await _refreshTokenRepository.RevokeAsync(request.Dto.RefreshToken);

            return Result.Success();
        }
    }
}
