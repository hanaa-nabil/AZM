using AZM.Application.Auth.Commands;
using AZM.Application.Auth.DTOs;
using AZM.Application.Common;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;

        public RegisterCommandHandler(
            UserManager<User> userManager,
            IUserRepository userRepository,
            ITokenService tokenService,
            IEmailService emailService,
            IOtpService otpService)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _emailService = emailService;
            _otpService = otpService;
        }

        public async Task<Result<AuthResponseDto>> Handle(
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
                return Result<AuthResponseDto>.Failure(
                    "Birth date cannot be today or in the future.", 400);

            if (birthDate < today.AddYears(-100))
                return Result<AuthResponseDto>.Failure(
                    "Please enter a valid birth date.", 400);

            // 3. Duplicate email check
            if (await _userRepository.EmailExistsAsync(email))
                return Result<AuthResponseDto>.Failure(
                    "An account with this email already exists.", 409);

            // 4. Duplicate phone check
            if (await _userRepository.PhoneExistsAsync(dto.PhoneNumber))
                return Result<AuthResponseDto>.Failure(
                    "An account with this phone number already exists.", 409);

            // 5. Create the user — Identity handles password hashing
            var user = new User
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = dto.PhoneNumber,
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
                return Result<AuthResponseDto>.Failure(errors, 400);
            }

            // 6. Generate OTP and send it to their email
            var otp = await _otpService.GenerateAndStoreOtpAsync(email);
            await _emailService.SendOtpEmailAsync(email, firstName, otp);

            // 7. Generate JWT
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateJwtToken(user, roles);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Token = token,
                EmailConfirmed = false
            }, 201);
        }
    }
}