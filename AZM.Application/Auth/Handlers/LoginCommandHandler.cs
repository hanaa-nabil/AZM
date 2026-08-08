using AZM.Application.Auth.Commands;
using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public LoginCommandHandler(
            UserManager<User> userManager,
            IUserRepository userRepository,
            ITokenService tokenService,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<Result<AuthResponseDto>> Handle(
         LoginCommand request,
         CancellationToken cancellationToken)
        {
            var email = request.Dto.Email.Trim().ToLowerInvariant();

            var user = await _userRepository.GetByEmailAsync(email);
            if (user is null)
                return Result<AuthResponseDto>.Failure("Invalid email or password.", 401);

            if (user.IsGoogleAccount)
                return Result<AuthResponseDto>.Failure(
                    "This account uses Google Sign-In. Please sign in with Google.", 400);

            if (!user.IsActive)
                return Result<AuthResponseDto>.Failure("Your account has been deactivated.", 403);

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Dto.Password);
            if (!passwordValid)
                return Result<AuthResponseDto>.Failure("Invalid email or password.", 401);

            if (!user.EmailConfirmed)
            {
                return Result<AuthResponseDto>.Failure(
                    "Please verify your email before signing in.",
                    403,
                    new AuthResponseDto { EmailConfirmed = false });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var (token, expiresAtUtc) = _tokenService.GenerateJwtToken(user, roles);
            user.LastLoginAtUtc = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            if (!string.IsNullOrWhiteSpace(request.Dto.FcmToken))
                await _userRepository.UpdateFcmTokenAsync(user.Id, request.Dto.FcmToken);

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64)),
                ExpiresAtUtc = DateTime.UtcNow.AddDays(30)
            };
            await _refreshTokenRepository.AddAsync(refreshToken);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Token = token,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiresAtUtc = refreshToken.ExpiresAtUtc,
                ExpiresAtUtc = expiresAtUtc,
                TokenType = "Bearer",
                IsRegistrationComplete = true,
                RequiresPhone = false,
                EmailConfirmed = true,
                IsVerified = true,
                Message = "Login successful."
            });
        }
    }
}