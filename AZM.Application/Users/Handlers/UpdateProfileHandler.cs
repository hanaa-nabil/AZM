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
        private readonly IMediator _mediator;

        public UpdateProfileHandler(IUserRepository userRepository, IPhotoService photoService, IMediator mediator)
        {
            _userRepository = userRepository;
            _photoService = photoService;
            _mediator = mediator;
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

            // --- Sports: remove first, then add (avoids collision if same sport appears in both by mistake) ---
            if (dto.SportsToRemove is { Count: > 0 })
            {
                foreach (var sport in dto.SportsToRemove)
                    await _mediator.Send(new RemoveUserSportCommand(request.UserId, sport), cancellationToken);
            }

            if (dto.SportsToAdd is { Count: > 0 })
            {
                foreach (var sport in dto.SportsToAdd)
                    await _mediator.Send(new AddUserSportCommand(request.UserId, sport), cancellationToken);
            }

            // --- Photo: mutually exclusive — remove wins if both somehow set ---
            if (dto.RemovePhoto)
            {
                await _mediator.Send(new RemoveProfilePhotoCommand(request.UserId), cancellationToken);
            }
            else if (dto.Photo is { Length: > 0 })
            {
                await _mediator.Send(new UploadProfilePhotoCommand(request.UserId, dto.Photo), cancellationToken);
            }

            var updatedUser = await _userRepository.GetByIdWithDetailsAsync(request.UserId)
                ?? throw new KeyNotFoundException("User not found after update.");

            return new UserProfileDto
            {
                Id = updatedUser.Id,
                Email = updatedUser.Email,
                FirstName = updatedUser.FirstName,
                LastName = updatedUser.LastName,
                Username = updatedUser.UserName ?? string.Empty,
                Bio = updatedUser.Profile?.Bio,
                Location = updatedUser.Profile?.Location,
                ProfilePhotoUrl = updatedUser.ProfilePhotoUrl ?? _photoService.GetInitialsAvatarUrl(initials, updatedUser.Id),
                IsIdVerified = updatedUser.IsIdVerified,
                Sports = updatedUser.Sports.Select(s => s.Sport).ToList(),
                EventsJoinedCount = updatedUser.Profile?.EventsJoinedCount ?? 0,
                EventsCompletedCount = updatedUser.Profile?.EventsCompletedCount ?? 0,
                TotalDistanceMeters = updatedUser.Profile?.TotalDistanceMeters ?? 0
            };
        }
    }
}


