using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AZM.Api.Controllers
{
    [Route("api/events")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST /api/events — Organizer creates a new event
        [HttpPost]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var command = new CreateEventCommand
            {
                OrganizerId = userId,
                Title = request.Title,
                Description = request.Description,
                ScheduledAt = request.ScheduledAt,
                Difficulty = request.Difficulty,
                MaxParticipants = request.MaxParticipants,
                MeetingLat = request.MeetingLat,
                MeetingLng = request.MeetingLng,
                Waypoints = request.Waypoints   // list of { lat, lng }
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetEvent), new { id = result.Value.Id }, result.Value);
        }

        // GET /api/events/nearby?lat=xx&lng=yy&radius=10000
        [HttpGet("nearby")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNearby([FromQuery] double lat, [FromQuery] double lng, [FromQuery] int radius = 10000)
        {
            var query = new GetNearbyEventsQuery { Lat = lat, Lng = lng, RadiusMeters = radius };
            var result = await _mediator.Send(query);
            return Ok(result.Value);
        }

        // GET /api/events/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            var query = new GetEventByIdQuery { EventId = id };
            var result = await _mediator.Send(query);

            if (result.Value is null) return NotFound();
            return Ok(result.Value);
        }

        // POST /api/events/{id}/join
        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinEvent(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new JoinEventCommand { EventId = id, UserId = userId };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new { message = "Joined successfully" });
        }

        // DELETE /api/events/{id}/leave
        [HttpDelete("{id}/leave")]
        public async Task<IActionResult> LeaveEvent(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new LeaveEventCommand { EventId = id, UserId = userId };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new { message = "Left event" });
        }

        // POST /api/events/{id}/publish
        [HttpPost("{id}/publish")]
        [Authorize(Roles = "Organizer,Admin")]
        public async Task<IActionResult> PublishEvent(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var command = new PublishEventCommand { EventId = id, RequestingUserId = userId };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new { message = "Event published" });
        }
    }
}
