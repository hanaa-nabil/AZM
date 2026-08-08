using AZM.Application.Auth.Commands;
using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            UserManager<User> userManager,
            ITokenService tokenService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;
            _tokenService = tokenService;
        }

        public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var stored = await _refreshTokenRepository.GetValidTokenAsync(request.Dto.RefreshToken);
            if (stored is null)
                return Result<AuthResponseDto>.Failure("Invalid or expired refresh token.", 401);

            var user = await _userManager.FindByIdAsync(stored.UserId.ToString());
            if (user is null)
                return Result<AuthResponseDto>.Failure("User not found.", 401);

            // Rotate: revoke the old refresh token, issue a new access + refresh pair
            await _refreshTokenRepository.RevokeAsync(stored.Token);

            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiresAtUtc) = _tokenService.GenerateJwtToken(user, roles);

            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
                ExpiresAtUtc = DateTime.UtcNow.AddDays(30)
            };
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Token = token,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiresAtUtc = newRefreshToken.ExpiresAtUtc,
                ExpiresAtUtc = expiresAtUtc,
                TokenType = "Bearer",
                Message = "Token refreshed."
            });
        }
    }
}
