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
    public class FollowUserByUsernameCommandHandler : IRequestHandler<FollowUserByUsernameCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediator _mediator;

        public FollowUserByUsernameCommandHandler(IUserRepository userRepository, IMediator mediator)
        {
            _userRepository = userRepository;
            _mediator = mediator;
        }

        public async Task<Result> Handle(FollowUserByUsernameCommand request, CancellationToken cancellationToken)
        {
            var targetUser = await _userRepository.GetByUsernameWithDetailsAsync(request.Username);
            if (targetUser is null)
                return Result.Failure("User not found.", 404);

            // Delegate to the existing command so all the real logic
            // (self-follow check, already-following check, notification) lives in one place
            return await _mediator.Send(new FollowUserCommand(request.FollowerId, targetUser.Id), cancellationToken);
        }
    }
}
