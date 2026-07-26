using AZM.Application.DTOs.User;
using AZM.Application.Users.Queries;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Handlers
{
    public class GetMyProfileHandler : IRequestHandler<GetMyProfileQuery, UserProfileDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPhotoService _photoService;
        public GetMyProfileHandler(IUserRepository userRepository, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _photoService = photoService;
        }

        public async Task<UserProfileDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(request.UserId)
                ?? throw new KeyNotFoundException("User not found.");

            var initials = $"{(user.FirstName.Length > 0 ? user.FirstName[0] : ' ')}{(user.LastName.Length > 0 ? user.LastName[0] : ' ')}".Trim().ToUpperInvariant();


            return new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName ?? string.Empty,
                Bio = user.Profile?.Bio,
                ProfilePhotoUrl = user.ProfilePhotoUrl ?? _photoService.GetInitialsAvatarUrl(initials, user.Id),
                IsIdVerified = user.IsIdVerified,
                Sports = user.Sports.Select(s => s.Sport).ToList(),
                EventsJoinedCount = user.Profile?.EventsJoinedCount ?? 0,
                EventsCompletedCount = user.Profile?.EventsCompletedCount ?? 0,
                TotalDistanceMeters = user.Profile?.TotalDistanceMeters ?? 0,
                Location = user.Profile?.Location
            };
        }
    }
}
