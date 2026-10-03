using AZM.Application.Squad.Commands;
using AZM.Application.Squads.Commands;
using AZM.Application.Squads.Queries;
using AZM.Application.Squads.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace AZM.Api.Controllers
{
    [Route("api/squads")]
    [ApiController]
    public class SquadsController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SquadsController(IMediator mediator) => _mediator = mediator;

        private Guid CurrentUserId =>
            Guid.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? throw new UnauthorizedAccessException("User id claim missing."));

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateSquadRequest request)
        {
            var result = await _mediator.Send(new CreateSquadCommand(
                CurrentUserId, request.Name, request.Bio, request.Sports,
                request.Latitude, request.Longitude, request.LocationName,
                request.Privacy, request.CoverImageUrl));

            return result.IsSuccess ? Ok(new { id = result.Data }) : BadRequest(result.Error);
        }

        [HttpGet("mine")]
        [Authorize]
        public async Task<IActionResult> GetMine()
        {
            var result = await _mediator.Send(new GetMySquadsQuery(CurrentUserId));
            return Ok(result);
        }

        [HttpGet("nearby")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNearby([FromQuery] double latitude, [FromQuery] double longitude, [FromQuery] double radiusKm = 10)
        {
            var result = await _mediator.Send(new GetNearbySquadsQuery(latitude, longitude, radiusKm));
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRoster(Guid id)
        {
            var result = await _mediator.Send(new GetSquadRosterQuery(id));
            return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
        }

        [HttpPost("{id:guid}/join")]
        [Authorize]
        public async Task<IActionResult> Join(Guid id)
        {
            var result = await _mediator.Send(new RequestToJoinSquadCommand(id, CurrentUserId));
            return result.IsSuccess ? Ok(new { message = "Request sent." }) : BadRequest(result.Error);
        }

        [HttpPost("{id:guid}/members/{userId:guid}/approve")]
        [Authorize]
        public async Task<IActionResult> Approve(Guid id, Guid userId)
        {
            var result = await _mediator.Send(new ApproveSquadRequestCommand(id, userId, CurrentUserId));
            return result.IsSuccess ? Ok(new { message = "Approved." }) : StatusCode(result.StatusCode, new { error = result.Error });
        }

        [HttpDelete("{id:guid}/members/{userId:guid}")]
        [Authorize]
        public async Task<IActionResult> RemoveMember(Guid id, Guid userId)
        {
            var result = await _mediator.Send(new RemoveSquadMemberCommand(id, userId, CurrentUserId));
            return result.IsSuccess ? Ok(new { message = "Removed." }) : StatusCode(result.StatusCode, new { error = result.Error });
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteSquadCommand(id, CurrentUserId));
            return result.IsSuccess ? NoContent() : StatusCode(result.StatusCode, new { error = result.Error });
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            var result = await _mediator.Send(new SearchSquadsQuery(q));
            return Ok(result);
        }
    }
}
