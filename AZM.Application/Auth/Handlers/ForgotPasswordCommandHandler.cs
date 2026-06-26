using AZM.Application.Auth.Commands;
using AZM.Application.Common;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Auth.Handlers
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(
            IUserRepository userRepository,
            IOtpService otpService,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _otpService = otpService;
            _emailService = emailService;
        }

        public async Task<Result> Handle(
            ForgotPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var email = request.Dto.Email.Trim().ToLowerInvariant();

            var user = await _userRepository.GetByEmailAsync(email);

            // Always return success to avoid leaking whether the email is registered.
            // Only send the OTP if the account actually exists and is a password account.
            if (user is not null && !user.IsGoogleAccount && user.IsActive)
            {
                // Reuse the OTP cooldown guard already built into IOtpService
                var canSend = await _otpService.IsResendAllowedAsync(email);
                if (!canSend)
                    return Result.Failure(
                        "Please wait 60 seconds before requesting another code.", 429);

                var otp = await _otpService.GenerateAndStoreOtpAsync(email);
                await _emailService.SendPasswordResetOtpAsync(email, user.FirstName, otp);
            }

            return Result.Success();
        }
    }
}