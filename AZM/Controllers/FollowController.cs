using AZM.Application.Follows.Commands;
using AZM.Application.Follows.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace AZM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FollowController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FollowController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? throw new UnauthorizedAccessException("User id claim missing."));

        [HttpPost("{userId:guid}")]
        public async Task<IActionResult> Follow(Guid userId)
        {
            var result = await _mediator.Send(new FollowUserCommand(CurrentUserId, userId));
            return result.IsSuccess
                ? Ok(new { message = "Followed successfully." })
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> Unfollow(Guid userId)
        {
            var result = await _mediator.Send(new UnfollowUserCommand(CurrentUserId, userId));
            return result.IsSuccess
                ? Ok(new { message = "Unfollowed successfully." })
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        [HttpGet("followers")]
        public async Task<IActionResult> GetMyFollowers()
        {
            var result = await _mediator.Send(new GetFollowersQuery(CurrentUserId));
            return Ok(result);
        }

        [HttpGet("following")]
        public async Task<IActionResult> GetMyFollowing()
        {
            var result = await _mediator.Send(new GetFollowingQuery(CurrentUserId));
            return Ok(result);
        }

        [HttpGet("followers/{userId:guid}")]
        public async Task<IActionResult> GetFollowersOf(Guid userId)
        {
            var result = await _mediator.Send(new GetFollowersQuery(userId));
            return Ok(result);
        }
    }
}
