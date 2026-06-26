using AZM.Application.Auth.Commands;
using AZM.Application.Common;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IOtpService _otpService;

        public ResetPasswordCommandHandler(
            UserManager<User> userManager,
            IUserRepository userRepository,
            IOtpService otpService)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _otpService = otpService;
        }

        public async Task<Result> Handle(
            ResetPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var email = request.Dto.Email.Trim().ToLowerInvariant();

            // 1. Validate OTP first (keeps the error message generic on bad email)
            var otpValid = await _otpService.ValidateOtpAsync(email, request.Dto.Otp);
            if (!otpValid)
                return Result.Failure("Invalid or expired reset code.", 400);

            // 2. Load the user
            var user = await _userRepository.GetByEmailAsync(email);
            if (user is null || user.IsGoogleAccount || !user.IsActive)
                return Result.Failure("Invalid or expired reset code.", 400);

            // 3. Replace the password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.Dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(" ", result.Errors.Select(e => e.Description));
                return Result.Failure(errors, 400);
            }

            // 4. Invalidate all remaining OTPs for this email
            await _otpService.InvalidateAllOtpsForEmailAsync(email);

            return Result.Success();
        }
    }
}