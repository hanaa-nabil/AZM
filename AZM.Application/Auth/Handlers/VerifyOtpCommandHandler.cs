using AZM.Application.Auth.Commands;
using AZM.Application.Common;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, Result>
    {
        private readonly UserManager<User> _userManager;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;

        public VerifyOtpCommandHandler(
            UserManager<User> userManager,
            IOtpService otpService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _otpService = otpService;
            _emailService = emailService;
        }

        public async Task<Result> Handle(
            VerifyOtpCommand request,
            CancellationToken cancellationToken)
        {
            var email = request.Dto.Email.Trim().ToLowerInvariant();
            var code = request.Dto.Code.Trim();

            // 1. Find the user
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result.Failure("No account found with this email.", 404);

            // 2. Check if already verified
            if (user.EmailConfirmed)
                return Result.Failure("This email is already verified.", 400);

            // 3. Validate the OTP
            var isValid = await _otpService.ValidateOtpAsync(email, code);
            if (!isValid)
                return Result.Failure("Invalid or expired OTP.", 400);

            // 4. Mark email as confirmed
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            // 5. Send welcome email
            await _emailService.SendWelcomeEmailAsync(email, user.FirstName);

            return Result.Success();
        }
    }
}