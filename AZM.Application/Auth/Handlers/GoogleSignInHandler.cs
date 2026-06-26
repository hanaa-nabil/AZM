using AZM.Application.Auth.Commands;
using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class GoogleSignInHandler : IRequestHandler<GoogleSignInCommand, Result<AuthResponseDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ISocialAuthService _socialAuthService;
        private readonly ITokenService _tokenService;
        private readonly IUserRepository _userRepository;

        public GoogleSignInHandler(
            UserManager<User> userManager,
            ISocialAuthService socialAuthService,
            ITokenService tokenService,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _socialAuthService = socialAuthService;
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        public async Task<Result<AuthResponseDto>> Handle(
            GoogleSignInCommand request,
            CancellationToken cancellationToken)
        {
            // 1. Verify the Google token
            var googleUser = await _socialAuthService.VerifyGoogleTokenAsync(request.Dto.IdToken);
            if (googleUser is null)
                return Result<AuthResponseDto>.Failure("Invalid Google token.", 401);

            var email = googleUser.Email.Trim().ToLowerInvariant();

            // 2. Block if a password account already exists with this email
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser is not null && !existingUser.IsGoogleAccount)
                return Result<AuthResponseDto>.Failure(
                    "An account with this email already exists. Please sign in with your password.", 409);

            // 3. Existing Google account — sign in and return token
            if (existingUser is not null && existingUser.IsGoogleAccount)
            {
                var existingRoles = await _userManager.GetRolesAsync(existingUser);
                var existingToken = _tokenService.GenerateJwtToken(existingUser, existingRoles);

                return Result<AuthResponseDto>.Success(new AuthResponseDto
                {
                    UserId = existingUser.Id,
                    Email = existingUser.Email!,
                    FullName = existingUser.FullName,
                    Token = existingToken,
                    EmailConfirmed = existingUser.EmailConfirmed,
                    ProfilePhotoUrl = existingUser.ProfilePhotoUrl
                });
            }

            // 4. New Google user — create account, no token yet
            //    Must go through: complete-registration (phone) → complete-profile (sports/photo) → token
            var user = new User
            {
                UserName = email,
                Email = email,
                FirstName = googleUser.FirstName,
                LastName = googleUser.LastName,
                GoogleId = googleUser.SocialId,
                IsGoogleAccount = true,
                EmailConfirmed = true,
                IsPendingPhoneNumber = true,
                IsActive = true,
                Profile = new UserProfile()
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(" ", createResult.Errors.Select(e => e.Description));
                return Result<AuthResponseDto>.Failure(errors, 400);
            }

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Token = string.Empty,
                EmailConfirmed = true
            }, 201);
        }
    }
}