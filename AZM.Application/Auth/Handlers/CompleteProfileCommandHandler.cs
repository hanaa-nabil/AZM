using AZM.Application.Auth.Commands;
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

            // 2. Email must be verified before completing profile
            if (!user.EmailConfirmed)
                return Result<AuthResponseDto>.Failure(
                    "Please verify your email before completing your profile.", 400);

            // 3. Phone must be verified before completing profile
            if (!user.PhoneNumberConfirmed)
                return Result<AuthResponseDto>.Failure(
                    "Please verify your phone number before completing your profile.", 400);

            // 4. At least one sport required (DTO validation handles this but belt-and-suspenders)
            if (dto.Sports is null || dto.Sports.Count == 0)
                return Result<AuthResponseDto>.Failure(
                    "Please select at least one sport.", 400);


            // 5. Replace sports — delete existing first, then insert fresh
            await _userRepository.RemoveUserSportsAsync(user.Id);

            foreach (var sport in dto.Sports.Distinct())
            {
                user.Sports.Add(new UserSport
                {
                    UserId = user.Id,
                    Sport = sport
                });
            }

            // 6. Upload photo if provided
            if (!string.IsNullOrWhiteSpace(dto.PhotoBase64))
            {
                try
                {
                    var publicId = $"azm/profiles/{user.Id}";
                    user.ProfilePhotoUrl = await _photoService.UploadPhotoAsync(
                        dto.PhotoBase64, publicId);
                }
                catch
                {
                    // Photo upload failure is non-fatal — profile is still complete
                    // Log here if you have a logger injected
                }
            }

            // 7. Persist
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(" ", updateResult.Errors.Select(e => e.Description));
                return Result<AuthResponseDto>.Failure(errors, 400);
            }

            // 8. Issue JWT — this is the only place a token is ever issued
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateJwtToken(user, roles);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Token = token,
                EmailConfirmed = user.EmailConfirmed,
                ProfilePhotoUrl = user.ProfilePhotoUrl
            });
        }
    }
}