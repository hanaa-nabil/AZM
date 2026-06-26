using AZM.Application.Auth.Commands;
using AZM.Application.Auth.DTOs;
using AZM.Application.Auth.DTOs.Auth;
using AZM.Application.Common;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AZM.Application.Auth.Handlers
{
    public class CompleteProfileCommandHandler
        : IRequestHandler<CompleteProfileCommand, Result<AuthResponseDto>>
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IPhotoService _photoService;

        public CompleteProfileCommandHandler(
            UserManager<User> userManager,
            IUserRepository userRepository,
            ITokenService tokenService,
            IPhotoService photoService)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _photoService = photoService;
        }

        public async Task<Result<AuthResponseDto>> Handle(
            CompleteProfileCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // 1. Find the user
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user is null)
                return Result<AuthResponseDto>.Failure("No account found.", 404);

            // 2. Must have a phone number before completing profile
            if (string.IsNullOrEmpty(user.PhoneNumber))
                return Result<AuthResponseDto>.Failure(
                    "Please add your phone number before completing your profile.", 400);

            // 3. Validate sports list
            if (dto.Sports is null || dto.Sports.Count == 0)
                return Result<AuthResponseDto>.Failure(
                    "Please select at least one sport.", 400);

            var validSports = Enum.GetValues<Domain.Enums.Sport>();
            var invalidSports = dto.Sports.Except(validSports).ToList();
            if (invalidSports.Count > 0)
                return Result<AuthResponseDto>.Failure(
                    $"Invalid sport value(s): {string.Join(", ", invalidSports)}", 400);

            // 4. Save favourite sports (remove old ones first to handle re-submissions)
            user.Sports.Clear();
            foreach (var sport in dto.Sports.Distinct())
            {
                user.Sports.Add(new UserSport
                {
                    UserId = user.Id,
                    Sport = sport
                });
            }

            // 5. Upload profile photo if provided
            if (!string.IsNullOrWhiteSpace(dto.PhotoBase64))
            {
                try
                {
                    var photoUrl = await _photoService.UploadPhotoAsync(
                        dto.PhotoBase64,
                        publicId: user.Id   // use userId as the Cloudinary public ID
                    );
                    user.ProfilePhotoUrl = photoUrl;
                }
                catch (Exception ex)
                {
                    // Photo upload failure should not block registration
                    // The frontend will show initials as fallback
                    _ = ex; // suppress unused variable warning
                }
            }

            // 6. Persist changes
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(" ", updateResult.Errors.Select(e => e.Description));
                return Result<AuthResponseDto>.Failure(errors, 400);
            }

            // 7. Registration fully complete — issue JWT
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateJwtToken(user, roles);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Token = token,
                EmailConfirmed = true,
                ProfilePhotoUrl = user.ProfilePhotoUrl
            });
        }
    }
}