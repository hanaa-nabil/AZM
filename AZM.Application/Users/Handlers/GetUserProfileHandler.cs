using AZM.Application.DTOs.User;
using AZM.Application.Users.Queries;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Users.Handlers
{
    public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IFollowRepository _followRepository;

        public GetUserProfileHandler(IUserRepository userRepository, IFollowRepository followRepository)
        {
            _userRepository = userRepository;
            _followRepository = followRepository;
        }

        public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken ct)
        {
            var user = await _userRepository.GetByIdWithDetailsAsync(request.TargetUserId)
                ?? throw new KeyNotFoundException("User not found.");

            var profile = user.Profile
                ?? throw new InvalidOperationException("User has no profile.");

            var followersCount = await _followRepository.GetFollowersCountAsync(user.Id);
            var followingCount = await _followRepository.GetFollowingCountAsync(user.Id);
            bool? isFollowedByMe = request.ViewerId.HasValue
                ? await _followRepository.IsFollowingAsync(request.ViewerId.Value, user.Id)
                : null;

            return new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Bio = profile.Bio,
                ProfilePhotoUrl = user.ProfilePhotoUrl,
                BirthDate = user.BirthDate,
                IsIdVerified = user.IsIdVerified,
                Sports = user.Sports.Select(us => us.Sport).ToList(),
                Location = profile.Location,
                EventsJoinedCount = profile.EventsJoinedCount,
                EventsCompletedCount = profile.EventsCompletedCount,
                TotalDistanceMeters = profile.TotalDistanceMeters,
                Gender = user.Gender,
                CreatedAtUtc = user.CreatedAtUtc,
                FollowersCount = followersCount,
                FollowingCount = followingCount,
                IsFollowedByMe = isFollowedByMe
            };
        }
    }
}
