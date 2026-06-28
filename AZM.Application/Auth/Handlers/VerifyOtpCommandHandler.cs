using AZM.Application.Auth.Commands;
using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, Result<VerifyOtpResponseDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;

        public VerifyOtpCommandHandler(
            UserManager<User> userManager,
            IOtpService otpService,
            IEmailService emailService,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _otpService = otpService;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        public async Task<Result<VerifyOtpResponseDto>> Handle(
            VerifyOtpCommand request,
            CancellationToken cancellationToken)
        {
            var email = request.Dto.Email.Trim().ToLowerInvariant();
            var code = request.Dto.Code.Trim();

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return Result<VerifyOtpResponseDto>.Failure("No account found with this email.", 404);

            if (user.EmailConfirmed)
                return Result<VerifyOtpResponseDto>.Failure("This email is already verified.", 400);

            var isValid = await _otpService.ValidateOtpAsync(email, code);
            if (!isValid)
                return Result<VerifyOtpResponseDto>.Failure("Invalid or expired OTP.", 400);

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            await _emailService.SendWelcomeEmailAsync(email, user.FirstName);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateJwtToken(user, roles);

            return Result<VerifyOtpResponseDto>.Success(new VerifyOtpResponseDto
            {
                Message = "Email verified successfully.",
                Token = token,
                UserId = user.Id.ToString()
            });
        }
    }
}
