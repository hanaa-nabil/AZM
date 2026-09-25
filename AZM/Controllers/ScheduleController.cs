using AZM.Application.Events.Commands;
using AZM.Application.Events.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AZM.Api.Controllers
{
    [ApiController]
    [Route("api/schedule")]
    [Authorize]
    public class ScheduleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScheduleController(IMediator mediator) => _mediator = mediator;

        private Guid CurrentUserId =>
            Guid.TryParse(User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var id)
                ? id
                : throw new UnauthorizedAccessException("User id claim missing.");

        [HttpGet("attending")]
        public async Task<IActionResult> GetAttending()
        {
            var result = await _mediator.Send(new GetMyJoinedEventsQuery(CurrentUserId));
            if (!result.IsSuccess) return BadRequest(new { message = result.Error });

            var upcoming = result.Data!
                .Where(e => e.EventDate >= DateTime.UtcNow && e.Status == "Upcoming")
                .OrderBy(e => e.EventDate);

            return Ok(upcoming);
        }

        [HttpGet("hosted")]
        public async Task<IActionResult> GetHosted()
        {
            var result = await _mediator.Send(new GetOrganizerEventsQuery(CurrentUserId, CurrentUserId));
            if (!result.IsSuccess) return BadRequest(new { message = result.Error });

            var all = result.Data!
                .OrderByDescending(e => e.EventDate);

            return Ok(all);
        }
    }
}