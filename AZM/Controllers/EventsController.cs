using AZM.Api.Requests;
using AZM.Application.Events.Commands;
using AZM.Application.Events.Queries;
using AZM.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AZM.Api.Controllers
{
    [ApiController]
    [Route("api/events")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EventsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST /api/events
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var command = new CreateEventCommand
            {
                CreatedByUserId = userId!,
                Title = request.Title,
                Description = request.Description,
                StartAtUtc = request.StartAtUtc,
                Difficulty = request.Difficulty,
                MaxParticipants = request.MaxParticipants,
                MeetingLat = request.MeetingLat,
                MeetingLng = request.MeetingLng,
                MeetingAddress = request.MeetingAddress,
                SportType = request.SportType
            };

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetEvent), new { id = result.Data!.Id }, result.Data);
        }

        // GET /api/events/nearby?lat=xx&lng=yy&radius=10000&sportType=Running
        [HttpGet("nearby")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNearby(
            [FromQuery] double lat,
            [FromQuery] double lng,
            [FromQuery] int radius = 10000,
            [FromQuery] SportType? sportType = null)
        {
            var query = new GetNearbyEventsQuery
            {
                Lat = lat,
                Lng = lng,
                RadiusMeters = radius,
                SportType = sportType
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // GET /api/events/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            var result = await _mediator.Send(new GetEventByIdQuery { EventId = id });
            if (result is null) return NotFound();
            return Ok(result);
        }

        // GET /api/events/{id}/participants
        [HttpGet("{id}/participants")]
        public async Task<IActionResult> GetParticipants(Guid id)
        {
            var ev = await _mediator.Send(new GetEventByIdQuery { EventId = id });
            if (ev is null) return NotFound();
            return Ok(ev.Participants);
        }

        // POST /api/events/{id}/join
        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinEvent(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var command = new JoinEventCommand { EventId = id, UserId = userId! };
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

            var command = new LeaveEventCommand { EventId = id, UserId = userId! };
            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return BadRequest(result.Error);

            return Ok(new { message = "Left event" });
        }
    }
}