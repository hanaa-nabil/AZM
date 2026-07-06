using AZM.Application.DTOs.User;
using AZM.Application.Users.Commands;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Users.Handlers
{
    public class UpdateProfileHandler : IRequestHandler<UpdateProfileCommand, UserProfileDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhotoService _photoService;
        public UpdateProfileHandler(IUserRepository userRepository,IPhotoService photoService)
        {
            _userRepository = userRepository;
            _photoService = photoService;
        }

        public async Task<UserProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(request.UserId)
                ?? throw new KeyNotFoundException("User not found.");
            var initials = $"{(user.FirstName.Length > 0 ? user.FirstName[0] : ' ')}{(user.LastName.Length > 0 ? user.LastName[0] : ' ')}".Trim().ToUpperInvariant();
            var dto = request.Request;

            if (!string.IsNullOrWhiteSpace(dto.FirstName))
                user.FirstName = dto.FirstName;

            if (!string.IsNullOrWhiteSpace(dto.LastName))
                user.LastName = dto.LastName;

            if (!string.IsNullOrWhiteSpace(dto.Username) && dto.Username != user.UserName)
            {
                var exists = await _userRepository.UsernameExistsAsync(dto.Username);
                if (exists)
                    throw new InvalidOperationException("Username is already taken.");

                var success = await _userRepository.UpdateUsernameAsync(user.Id, dto.Username);
                if (!success)
                    throw new InvalidOperationException("Failed to update username.");
            }

            user.Profile ??= new UserProfile { UserId = user.Id };

            if (dto.Bio is not null)
                user.Profile.Bio = dto.Bio;

            if (dto.Location is not null)
                user.Profile.Location = dto.Location;

            user.Profile.UpdatedAtUtc = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            var updatedUser = await _userRepository.GetByIdWithDetailsAsync(request.UserId)
                ?? throw new KeyNotFoundException("User not found after update.");

            return new UserProfileDto
            {
                Id = updatedUser.Id,
                FullName = updatedUser.FullName,
                Username = updatedUser.UserName ?? string.Empty,
                Bio = updatedUser.Profile?.Bio,
                Location = updatedUser.Profile?.Location,
                ProfilePhotoUrl = user.ProfilePhotoUrl ?? _photoService.GetInitialsAvatarUrl(initials, user.Id),
                IsIdVerified = updatedUser.IsIdVerified,
                Sports = updatedUser.Sports.Select(s => s.Sport).ToList(),
                EventsJoinedCount = updatedUser.Profile?.EventsJoinedCount ?? 0,
                EventsCompletedCount = updatedUser.Profile?.EventsCompletedCount ?? 0,
                TotalDistanceMeters = updatedUser.Profile?.TotalDistanceMeters ?? 0
            };
        }
    }
}

