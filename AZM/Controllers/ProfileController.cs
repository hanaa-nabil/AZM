using AZM.Application.DTOs.User;
using AZM.Application.Users.Commands;
using AZM.Application.Users.Queries;
using AZM.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        // ASSUMPTION: matches the JwtRegisteredClaimNames.Sub fix you applied in EventsController
        // for CurrentUserId. Swap this to whatever shared helper/base-controller property
        // you're using there so both controllers resolve the user id identically.
        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value
                ?? throw new UnauthorizedAccessException("User id claim missing."));

        [HttpGet("me")]
        public async Task<ActionResult<UserProfileDto>> GetMyProfile()
        {
            var result = await _mediator.Send(new GetMyProfileQuery(CurrentUserId));
            return Ok(result);
        }

        [HttpPut("me")]
        public async Task<ActionResult<UserProfileDto>> UpdateProfile([FromBody] UpdateProfileRequestDto request)
        {
            var result = await _mediator.Send(new UpdateProfileCommand(CurrentUserId, request));
            return Ok(result);
        }

        [HttpPost("me/sports")]
        public async Task<IActionResult> AddSport(Sport sport)
        {
            if (!Enum.IsDefined(typeof(Sport), sport))
             return BadRequest("Invalid sport value.");

            await _mediator.Send(new AddUserSportCommand(CurrentUserId, sport));
            return NoContent();
        }

        [HttpDelete("me/sports/{sport}")]
        public async Task<IActionResult> RemoveSport(Sport sport)
        {

            if (!Enum.IsDefined(typeof(Sport), sport))
                return BadRequest("Invalid sport value.");

            await _mediator.Send(new RemoveUserSportCommand(CurrentUserId, sport));
            return NoContent();
        }

        [HttpPost("me/photo")]
        [RequestSizeLimit(5_000_000)] 
        public async Task<IActionResult> UploadPhoto(IFormFile photo)
        {
            if (photo is null || photo.Length == 0)
                return BadRequest("No photo provided.");

            var url = await _mediator.Send(new UploadProfilePhotoCommand(CurrentUserId, photo));
            return Ok(new { profilePhotoUrl = url });
        }

        [HttpDelete("me/photo")]
        public async Task<IActionResult> RemovePhoto()
        {
            await _mediator.Send(new RemoveProfilePhotoCommand(CurrentUserId));
            return NoContent();
        }
    }
}
