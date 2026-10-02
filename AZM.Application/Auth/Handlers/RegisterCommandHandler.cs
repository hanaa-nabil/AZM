using AZM.Application.Auth.Commands;
using AZM.Application.Common;
using AZM.Application.DTOs.Auth;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponseDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;

        public RegisterCommandHandler(
            UserManager<User> userManager,
            IUserRepository userRepository,
            IEmailService emailService,
            IOtpService otpService)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _emailService = emailService;
            _otpService = otpService;
        }
        public async Task<Result<RegisterResponseDto>> Handle(
    RegisterCommand request,
    CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            var email = dto.Email.Trim().ToLowerInvariant();
            var firstName = dto.FirstName.Trim();
            var lastName = dto.LastName.Trim();
            var username = dto.Username.Trim();

            var user = new User
            {
                UserName = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender,
                EmailConfirmed = false,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                Profile = new UserProfile()
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(" ", createResult.Errors.Select(e => e.Description));
                return Result<RegisterResponseDto>.Failure(errors, 400);
            }

            try
            {
                var otp = await _otpService.GenerateAndStoreOtpAsync(email);
                await _emailService.SendOtpEmailAsync(email, firstName, otp);
            }
            catch (Exception ex)
            {
                await _userManager.DeleteAsync(user);
                return Result<RegisterResponseDto>.Failure($"Failed to send verification email: {ex.Message}", 500);
            }

            return Result<RegisterResponseDto>.Success(new RegisterResponseDto
            {
                UserId = user.Id.ToString(),
                Email = user.Email!,
                FirstName = firstName,
                LastName = lastName,
                BirthDate = dto.BirthDate,
                EmailVerificationRequired = true,
                PhoneNumberRequired = true,
                Message = "Please check your email for the verification code."
            }, 201);
        }
    }
}