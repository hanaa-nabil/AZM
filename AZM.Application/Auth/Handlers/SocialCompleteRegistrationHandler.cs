using AZM.Application.Auth.Commands;
using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class SocialCompleteRegistrationHandler
        : IRequestHandler<SocialCompleteRegistrationCommand, Result<RegisterResponseDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public SocialCompleteRegistrationHandler(
            UserManager<User> userManager,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task<Result<RegisterResponseDto>> Handle(
            SocialCompleteRegistrationCommand request,
            CancellationToken cancellationToken)
        {
            var email = request.Dto.Email.Trim().ToLowerInvariant();
            var phoneNumber = request.Dto.PhoneNumber.Trim();

            // 1. Find the user
            var user = await _userRepository.GetByEmailAsync(email);
            if (user is null)
                return Result<RegisterResponseDto>.Failure("No account found with this email.", 404);

            // 2. Make sure this is actually a Google account
            if (!user.IsGoogleAccount)
                return Result<RegisterResponseDto>.Failure(
                    "This endpoint is only for social accounts.", 400);

            // 3. Make sure they haven't already completed registration
            if (!user.IsPendingPhoneNumber)
                return Result<RegisterResponseDto>.Failure(
                    "Registration is already complete.", 400);

            // 4. Check phone number is not already taken
            if (await _userRepository.PhoneExistsAsync(phoneNumber))
                return Result<RegisterResponseDto>.Failure(
                    "An account with this phone number already exists.", 409);

            // 5. Update phone number and mark phone step as complete
            user.PhoneNumber = phoneNumber;
            user.IsPendingPhoneNumber = false;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(" ", updateResult.Errors.Select(e => e.Description));
                return Result<RegisterResponseDto>.Failure(errors, 400);
            }

            // 6. Phone saved — proceed to complete-profile to pick sports, add photo, and get token
            return Result<RegisterResponseDto>.Success(new RegisterResponseDto
            {
                UserId = user.Id,
                Email = user.Email!
            });
        }
    }
}