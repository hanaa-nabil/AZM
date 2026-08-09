using AZM.Application.DTOs.Follow;
using AZM.Application.Follows.Queries;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Follows.Handlers
{
    public class GetFollowersHandler : IRequestHandler<GetFollowersQuery, List<FollowUserDto>>
    {
        private readonly IFollowRepository _followRepository;

        public GetFollowersHandler(IFollowRepository followRepository)
        {
            _followRepository = followRepository;
        }

        public async Task<List<FollowUserDto>> Handle(GetFollowersQuery request, CancellationToken cancellationToken)
        {
            var followers = await _followRepository.GetFollowersAsync(request.UserId);
            return followers.Select(u => new FollowUserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Username = u.UserName ?? string.Empty,
                ProfilePhotoUrl = u.ProfilePhotoUrl
            }).ToList();
        }
    }
}
