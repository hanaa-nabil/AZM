using AZM.Application.Auth.Commands;
using AZM.Application.Auth.DTOs;
using AZM.Application.Common;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class SocialCompleteRegistrationHandler : IRequestHandler<SocialCompleteRegistrationCommand, Result<AuthResponseDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public SocialCompleteRegistrationHandler(
            UserManager<User> userManager,
            IUserRepository userRepository,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<Result<AuthResponseDto>> Handle(
            SocialCompleteRegistrationCommand request,
            CancellationToken cancellationToken)
        {
            var email = request.Dto.Email.Trim().ToLowerInvariant();
            var phoneNumber = request.Dto.PhoneNumber.Trim();

            // 1. Find the user
            var user = await _userRepository.GetByEmailAsync(email);
            if (user is null)
                return Result<AuthResponseDto>.Failure("No account found with this email.", 404);

            // 2. Make sure this is actually a Google account
            if (!user.IsGoogleAccount)
                return Result<AuthResponseDto>.Failure("This endpoint is only for social accounts.", 400);

            // 3. Make sure they haven't already completed registration
            if (!user.IsPendingPhoneNumber)
                return Result<AuthResponseDto>.Failure("Registration is already complete.", 400);

            // 4. Check phone number is not already taken
            if (await _userRepository.PhoneExistsAsync(phoneNumber))
                return Result<AuthResponseDto>.Failure(
                    "An account with this phone number already exists.", 409);

            // 5. Update phone number and mark registration as complete
            user.PhoneNumber = phoneNumber;
            user.IsPendingPhoneNumber = false;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(" ", updateResult.Errors.Select(e => e.Description));
                return Result<AuthResponseDto>.Failure(errors, 400);
            }

            // 6. Generate fresh JWT
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateJwtToken(user, roles);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Token = token,
                EmailConfirmed = true
            });
        }
    }
}