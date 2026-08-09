using AZM.Application.Common;
using AZM.Application.Follows.Commands;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Follows.Handlers
{
    public class UnfollowUserCommandHandler : IRequestHandler<UnfollowUserCommand, Result>
    {
        private readonly IFollowRepository _followRepository;

        public UnfollowUserCommandHandler(IFollowRepository followRepository)
        {
            _followRepository = followRepository;
        }

        public async Task<Result> Handle(UnfollowUserCommand request, CancellationToken cancellationToken)
        {
            await _followRepository.UnfollowAsync(request.FollowerId, request.FollowingId);
            return Result.Success();
        }
    }
}
