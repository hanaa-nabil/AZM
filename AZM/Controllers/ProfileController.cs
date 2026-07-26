using AZM.Application.DTOs.User;
using AZM.Application.Users.Commands;
using AZM.Application.Users.Queries;
using AZM.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace AZM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? throw new UnauthorizedAccessException("User id claim missing."));

        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetMyProfile()
        {
            var result = await _mediator.Send(new GetMyProfileQuery(CurrentUserId));
            return Ok(result);
        }

        [HttpPut("me")]
        [RequestSizeLimit(5_000_000)] 
        public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromForm] UpdateProfileRequestDto request)
        {
            var result = await _mediator.Send(new UpdateProfileCommand(CurrentUserId, request));
            return Ok(result);
        }

        [HttpGet("streak")]
        public async Task<IActionResult> GetMyStreak()
        {
            var result = await _mediator.Send(new GetMyStreakQuery(CurrentUserId));
            return Ok(result);
        }

        [HttpPost("streak/freeze")]
        public async Task<IActionResult> UseStreakFreeze()
        {
            var success = await _mediator.Send(new UseStreakFreezeCommand(CurrentUserId));
            return success ? Ok() : BadRequest(new { message = "No streak freezes available." });
        }

        [HttpGet("achievements")]
        public async Task<IActionResult> GetMyAchievements()
        {
            var result = await _mediator.Send(new GetMyAchievementsQuery(CurrentUserId));
            return Ok(result);
        }
    }
}