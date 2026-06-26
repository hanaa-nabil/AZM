using AZM.Application.Auth.Commands;
using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
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

        public LoginCommandHandler(
            UserManager<User> userManager,
            IUserRepository userRepository,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<Result<AuthResponseDto>> Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
        {
            var email = request.Dto.Email.Trim().ToLowerInvariant();

            // 1. Find user
            var user = await _userRepository.GetByEmailAsync(email);
            if (user is null)
                return Result<AuthResponseDto>.Failure("Invalid email or password.", 401);

            // 2. Block Google-only accounts from password login
            if (user.IsGoogleAccount)
                return Result<AuthResponseDto>.Failure(
                    "This account uses Google Sign-In. Please sign in with Google.", 400);

            // 3. Check account is active
            if (!user.IsActive)
                return Result<AuthResponseDto>.Failure("Your account has been deactivated.", 403);

            // 4. Verify password
            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Dto.Password);
            if (!passwordValid)
                return Result<AuthResponseDto>.Failure("Invalid email or password.", 401);

            // 5. Require email confirmation before granting a token
            if (!user.EmailConfirmed)
                return Result<AuthResponseDto>.Failure(
                    "Please verify your email address before signing in.", 403);

            // 6. Issue JWT
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateJwtToken(user, roles);

            user.LastLoginAtUtc = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Token = token,
                EmailConfirmed = user.EmailConfirmed,
                ProfilePhotoUrl = user.ProfilePhotoUrl
            });
        }
    }
}