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
    public class GetFollowingHandler : IRequestHandler<GetFollowingQuery, List<FollowUserDto>>
    {
        private readonly IFollowRepository _followRepository;

        public GetFollowingHandler(IFollowRepository followRepository)
        {
            _followRepository = followRepository;
        }

        public async Task<List<FollowUserDto>> Handle(GetFollowingQuery request, CancellationToken cancellationToken)
        {
            var following = await _followRepository.GetFollowingAsync(request.UserId);
            return following.Select(u => new FollowUserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Username = u.UserName ?? string.Empty,
                ProfilePhotoUrl = u.ProfilePhotoUrl
            }).ToList();
        }
    }
}
