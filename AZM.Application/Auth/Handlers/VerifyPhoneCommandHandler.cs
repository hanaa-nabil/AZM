using AZM.Application.Auth.Commands;
using AZM.Application.Common;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class VerifyPhoneCommandHandler : IRequestHandler<VerifyPhoneCommand, Result>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IFirebaseAuthService _firebaseAuthService;

        public VerifyPhoneCommandHandler(
            UserManager<User> userManager,
            IUserRepository userRepository,
            IFirebaseAuthService firebaseAuthService)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _firebaseAuthService = firebaseAuthService;
        }

        public async Task<Result> Handle(
            VerifyPhoneCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // 1. Find the user
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user is null)
                return Result.Failure("No account found.", 404);

            // 2. Phone must exist (add-phone / complete-registration must come first)
            if (string.IsNullOrEmpty(user.PhoneNumber))
                return Result.Failure(
                    "Please add a phone number before verifying.", 400);

            // 3. Already verified — idempotent, just return success
            if (user.PhoneNumberConfirmed)
                return Result.Success();

            // 4. Validate the Firebase token and extract the phone number from it
            var verifiedPhone = await _firebaseAuthService.VerifyPhoneTokenAsync(
                dto.FirebaseIdToken);

            if (verifiedPhone is null)
                return Result.Failure("Invalid or expired verification token.", 400);

            // 5. Make sure the verified phone matches what the user submitted earlier
            if (verifiedPhone != user.PhoneNumber)
                return Result.Failure(
                    "Verified phone number does not match the registered number.", 400);

            // 6. Mark phone as confirmed
            user.PhoneNumberConfirmed = true;
            user.IsPendingPhoneVerification = false;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(" ", updateResult.Errors.Select(e => e.Description));
                return Result.Failure(errors, 400);
            }

            return Result.Success();
        }
    }
}