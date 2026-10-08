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
            if (!result.IsSuccess) return BadRequest(new { error = result.Error });

            var upcoming = result.Data!
                .Where(e => e.EventDate >= DateTime.UtcNow && (e.Status == "Upcoming" || e.Status == "Soon"))
                .OrderBy(e => e.EventDate);
            
            return Ok(upcoming);
        }
        /// <summary>
        /// "Hosting" tab: events the user organizes that are today or in the future.
        /// </summary>
        [HttpGet("hosting")]
        public async Task<IActionResult> GetHosting()
        {
            var result = await _mediator.Send(new GetOrganizerEventsQuery(CurrentUserId, CurrentUserId));
            if (!result.IsSuccess) return BadRequest(new { error = result.Error });

            var startOfToday = DateTime.UtcNow.Date;

            var upcoming = result.Data!
                .Where(e => e.EventDate >= startOfToday && e.Status != "Cancelled")
                .OrderBy(e => e.EventDate);

            return Ok(upcoming);
        }
    }
}