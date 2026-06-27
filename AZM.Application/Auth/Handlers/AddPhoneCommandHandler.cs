using AZM.Application.Auth.Commands;
using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class AddPhoneCommandHandler : IRequestHandler<AddPhoneCommand, Result<RegisterResponseDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public AddPhoneCommandHandler(
            UserManager<User> userManager,
            IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public async Task<Result<RegisterResponseDto>> Handle(
            AddPhoneCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // 1. Find the user by ID
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user is null)
                return Result<RegisterResponseDto>.Failure("No account found.", 404);

            // 2. Email must be verified before adding a phone number
            if (!user.EmailConfirmed)
                return Result<RegisterResponseDto>.Failure(
                    "Please verify your email before adding a phone number.", 400);

            // 3. Check phone number is not already taken by another account
            if (await _userRepository.PhoneExistsAsync(dto.PhoneNumber))
                return Result<RegisterResponseDto>.Failure(
                    "An account with this phone number already exists.", 409);

            // 4. Save the phone number
            //    Phone OTP verification will be handled by the frontend via Firebase.
            //    When integration is ready, uncomment the block below and verify
            //    the Firebase ID token before saving the phone number.
            //
            // ---------- PHONE VERIFICATION (uncomment when integrating frontend) ----------
            // var phoneVerified = await _phoneVerificationService.VerifyFirebaseTokenAsync(dto.FirebaseIdToken);
            // if (!phoneVerified)
            //     return Result<RegisterResponseDto>.Failure("Phone verification failed.", 400);
            // -------------------------------------------------------------------------------

            user.PhoneNumber = dto.PhoneNumber;
            user.IsPendingPhoneVerification = true;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(" ", updateResult.Errors.Select(e => e.Description));
                return Result<RegisterResponseDto>.Failure(errors, 400);
            }

            // 5. Phone saved — proceed to complete-profile to pick sports, add photo, and get token
            return Result<RegisterResponseDto>.Success(new RegisterResponseDto
            {
                UserId = user.Id.ToString(),
                Email = user.Email!
            });
        }
    }
}