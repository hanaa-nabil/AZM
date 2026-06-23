using AZM.Application.Auth.Commands;
using AZM.Application.Common;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, Result>
    {
        private readonly UserManager<User> _userManager;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;

        public ResendOtpCommandHandler(
            UserManager<User> userManager,
            IOtpService otpService,
            IEmailService emailService)
        {
            _userManager = userManager;
            _otpService = otpService;
            _emailService = emailService;
        }

        public async Task<Result> Handle(
            ResendOtpCommand request,
            CancellationToken cancellationToken)
        {
            var email = request.Dto.Email.Trim().ToLowerInvariant();

            // 1. Find the user
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result.Failure("No account found with this email.", 404);

            // 2. Check if already verified — no point resending
            if (user.EmailConfirmed)
                return Result.Failure("This email is already verified.", 400);

            // 3. Check cooldown — must wait 60 seconds between resend requests
            var allowed = await _otpService.IsResendAllowedAsync(email);
            if (!allowed)
                return Result.Failure("Please wait 60 seconds before requesting a new OTP.", 429);

            // 4. Generate a fresh OTP and send it
            var otp = await _otpService.GenerateAndStoreOtpAsync(email);
            await _emailService.SendOtpEmailAsync(email, user.FirstName, otp);

            return Result.Success();
        }
    }
}