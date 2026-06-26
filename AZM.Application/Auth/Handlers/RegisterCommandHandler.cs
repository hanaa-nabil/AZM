using AZM.Application.Auth.Commands;
using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
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

            // 1. Normalize
            var email = dto.Email.Trim().ToLowerInvariant();
            var firstName = dto.FirstName.Trim();
            var lastName = dto.LastName.Trim();

            // 2. Birthdate validation
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var birthDate = DateOnly.FromDateTime(dto.BirthDate);

            if (birthDate >= today)
                return Result<RegisterResponseDto>.Failure(
                    "Birth date cannot be today or in the future.", 400);

            if (birthDate < today.AddYears(-100))
                return Result<RegisterResponseDto>.Failure(
                    "Please enter a valid birth date.", 400);

            // 3. Duplicate email check
            if (await _userRepository.EmailExistsAsync(email))
                return Result<RegisterResponseDto>.Failure(
                    "An account with this email already exists.", 409);

            // 4. Create the user — phone number will be added in the next step
            var user = new User
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                BirthDate = dto.BirthDate,
                Gender = dto.Gender,
                EmailConfirmed = false,
                IsActive = true,
                Profile = new UserProfile()
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(" ", createResult.Errors.Select(e => e.Description));
                return Result<RegisterResponseDto>.Failure(errors, 400);
            }

            // 5. Generate OTP and send it to their email
            //    If the email send fails, delete the user so we don't leave an orphaned account
            try
            {
                var otp = await _otpService.GenerateAndStoreOtpAsync(email);
                await _emailService.SendOtpEmailAsync(email, firstName, otp);
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                return Result<RegisterResponseDto>.Failure(
                    "Failed to send verification email. Please try again.", 500);
            }

            // 6. Return userId + email only — JWT is issued after the full flow is complete
            return Result<RegisterResponseDto>.Success(new RegisterResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                EmailVerificationRequired = true,   // ← add this
                PhoneNumberRequired = true,          // ← add this
            Message = "Please check your email for the verification code."
            }, 201);
        }
    }
}