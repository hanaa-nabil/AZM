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
        private readonly IMediator _mediator;

        public UpdateProfileHandler(IUserRepository userRepository, IMediator mediator)
        {
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<UserProfileDto> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(request.UserId)
                ?? throw new KeyNotFoundException("User not found.");

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

            // Gender — no restriction, updates freely
            if (dto.Gender.HasValue)
                user.Gender = dto.Gender.Value;

            // BirthDate — throttled to once every 60 days
            if (dto.BirthDate.HasValue)
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                var newBirthDate = DateOnly.FromDateTime(dto.BirthDate.Value);

                if (newBirthDate >= today)
                    throw new InvalidOperationException("Birth date cannot be today or in the future.");

                if (newBirthDate < today.AddYears(-100))
                    throw new InvalidOperationException("Please enter a valid birth date.");

                if (user.LastBirthDateChangeUtc.HasValue)
                {
                    var nextAllowedChange = user.LastBirthDateChangeUtc.Value.AddDays(60);
                    if (DateTime.UtcNow < nextAllowedChange)
                    {
                        var daysRemaining = (nextAllowedChange - DateTime.UtcNow).Days;
                        throw new InvalidOperationException(
                            $"Birth date can only be changed once every 60 days. Try again in {daysRemaining} day(s).");
                    }
                }

                user.BirthDate = dto.BirthDate.Value;
                user.LastBirthDateChangeUtc = DateTime.UtcNow;
            }

            user.Profile ??= new UserProfile { UserId = user.Id };
            if (dto.Bio is not null)
                user.Profile.Bio = dto.Bio;
            if (dto.Location is not null)
                user.Profile.Location = dto.Location;
            user.Profile.UpdatedAtUtc = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

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
                ProfilePhotoUrl = updatedUser.ProfilePhotoUrl,
                IsIdVerified = updatedUser.IsIdVerified,
                Sports = updatedUser.Sports.Select(s => s.Sport).ToList(),
                EventsJoinedCount = updatedUser.Profile?.EventsJoinedCount ?? 0,
                EventsCompletedCount = updatedUser.Profile?.EventsCompletedCount ?? 0,
                TotalDistanceMeters = updatedUser.Profile?.TotalDistanceMeters ?? 0,
                BirthDate = updatedUser.BirthDate,
                Gender = updatedUser.Gender,
                CreatedAtUtc = updatedUser.CreatedAtUtc
            };
        }
    }
}


