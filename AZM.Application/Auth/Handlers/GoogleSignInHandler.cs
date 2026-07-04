using AZM.Application.Auth.Commands;
using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
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

            // 2. Look up existing account
            var existingUser = await _userRepository.GetByEmailAsync(email);

            if (existingUser is not null)
            {
                // Block password accounts from using Google sign-in
                if (!existingUser.IsGoogleAccount)
                    return Result<AuthResponseDto>.Failure(
                        "An account with this email already exists. Please sign in with your password.", 409);

                // Existing Google account — sign in and return token
                var existingRoles = await _userManager.GetRolesAsync(existingUser);
                var existingToken = _tokenService.GenerateJwtToken(existingUser, existingRoles);

                return Result<AuthResponseDto>.Success(new AuthResponseDto
                {
                    UserId = existingUser.Id,
                    Email = existingUser.Email!,
                    FullName = existingUser.FullName,
                    Token = existingToken,
                    EmailConfirmed = existingUser.EmailConfirmed,
                    ProfilePhotoUrl = existingUser.ProfilePhotoUrl,
                    IsRegistrationComplete = true
                });
            }

            // 3. New Google user — create account
            //    No token yet: must go through complete-registration (phone) → complete-profile → token
            var user = new User
            {
                UserName = email,
                Email = email,
                FirstName = googleUser.FirstName,
                LastName = googleUser.LastName,
                // GoogleId stored via ExternalId to keep ISocialAuthService provider-agnostic
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
                // Handle race condition: another request created this account between our lookup and create
                if (createResult.Errors.Any(e => e.Code == "DuplicateEmail" || e.Code == "DuplicateUserName"))
                    return Result<AuthResponseDto>.Failure(
                        "An account with this email already exists. Please sign in with your password.", 409);

                var errors = string.Join(" ", createResult.Errors.Select(e => e.Description));
                return Result<AuthResponseDto>.Failure(errors, 400);
            }

            // Assign default role so JWT role claims are populated on first full sign-in
            await _userManager.AddToRoleAsync(user, "User");

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Token = null,
                EmailConfirmed = true,
                IsRegistrationComplete = false,
                RequiresPhone = true
            }, 201);
        }
    }
}