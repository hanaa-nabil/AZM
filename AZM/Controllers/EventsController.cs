using AZM.Api.Requests;
using AZM.Application.Auth.DTOs.Event;
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

        public EventsController(IMediator mediator) => _mediator = mediator;

        private Guid? CurrentUserId =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        // ── Feed ──────────────────────────────────────────────────────────────────

        /// <summary>
        /// Get paginated event feed (like a post list). Shows participant count.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeed(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] SportType? sportType = null,
            [FromQuery] EventStatus? status = null)
        {
            var result = await _mediator.Send(
                new GetEventFeedQuery(CurrentUserId, page, pageSize, sportType, status));

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        // ── Nearby ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Get events near a location. Flutter sends user's lat/lng.
        /// </summary>
        [HttpGet("nearby")]
        [AllowAnonymous]
        public async Task<IActionResult> GetNearby(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusKm = 10)
        {
            var result = await _mediator.Send(
                new GetNearbyEventsQuery(latitude, longitude, radiusKm, CurrentUserId));

            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        // ── Detail ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Get full event detail including participants list.
        /// </summary>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetEventByIdQuery(id, CurrentUserId));
            return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
        }

        // ── Participants ──────────────────────────────────────────────────────────

        /// <summary>
        /// Get list of who joined an event. Called when user taps participant count.
        /// </summary>
        [HttpGet("{id:guid}/participants")]
        [AllowAnonymous]
        public async Task<IActionResult> GetParticipants(Guid id)
        {
            var result = await _mediator.Send(new GetEventParticipantsQuery(id));
            return result.IsSuccess ? Ok(result.Data) : NotFound(result.Error);
        }

        // ── My Events ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Get all events the authenticated user has joined.
        /// </summary>
        [HttpGet("me/joined")]
        [Authorize]
        public async Task<IActionResult> GetMyJoined()
        {
            var result = await _mediator.Send(new GetMyJoinedEventsQuery(CurrentUserId!.Value));
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }

        /// <summary>
        /// Get all events the authenticated user organized.
        /// </summary>
        [HttpGet("me/organized")]
        [Authorize]
        public async Task<IActionResult> GetMyOrganized()
        {
            var result = await _mediator.Send(
                new GetOrganizerEventsQuery(CurrentUserId!.Value, CurrentUserId));
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }


        // ── Create ─────────────────────────────────────────────────────────────────
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
        {
            // Map WaypointRequest → WaypointDto
            EventRouteDto? routeDto = null;
            if (request.Route is not null)
            {
                routeDto = new EventRouteDto(
                    request.Route.StartLatitude,
                    request.Route.StartLongitude,
                    request.Route.StartAddress,
                    request.Route.EndLatitude,
                    request.Route.EndLongitude,
                    request.Route.EndAddress,
                    request.Route.DistanceMeters,
                    request.Route.EstimatedDurationSeconds,
                    request.Route.Waypoints?
                        .Select(w => new WaypointDto(w.Order, w.Latitude, w.Longitude))
                        .ToList());
            }

            var cmd = new CreateEventCommand(
                request.Title, request.Description,
                request.SportType, request.DifficultyLevel,
                request.Latitude, request.Longitude, request.LocationName,
                request.EventDate, CurrentUserId!.Value,
                request.MaxParticipants, request.DistanceKm, request.CoverImageUrl,
                request.IsPublic, routeDto);

            var result = await _mediator.Send(cmd);
            if (!result.IsSuccess) return BadRequest(result.Error);
            return CreatedAtAction(nameof(GetById), new { id = result.Data }, new { id = result.Data });
        }

        // ── Update ─────────────────────────────────────────────────────────────────
        [HttpPut("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEventRequest request)
        {
            EventRouteDto? routeDto = null;
            if (request.Route is not null)
            {
                routeDto = new EventRouteDto(
                    request.Route.StartLatitude, request.Route.StartLongitude,
                    request.Route.StartAddress,
                    request.Route.EndLatitude, request.Route.EndLongitude,
                    request.Route.EndAddress,
                    request.Route.DistanceMeters, request.Route.EstimatedDurationSeconds,
                    request.Route.Waypoints?
                        .Select(w => new WaypointDto(w.Order, w.Latitude, w.Longitude))
                        .ToList());
            }

            var cmd = new UpdateEventCommand(
                id, CurrentUserId!.Value,
                request.Title, request.Description, request.DifficultyLevel,
                request.Latitude, request.Longitude, request.LocationName,
                request.EventDate, request.MaxParticipants,
                request.DistanceKm, request.CoverImageUrl,
                request.IsPublic, routeDto);

            var result = await _mediator.Send(cmd);
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        // ── Publish / Cancel ──────────────────────────────────────────────────────

        [HttpPost("{id:guid}/publish")]
        [Authorize]
        public async Task<IActionResult> Publish(Guid id)
        {
            var result = await _mediator.Send(new PublishEventCommand(id, CurrentUserId!.Value));
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        [HttpPost("{id:guid}/cancel")]
        [Authorize]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var result = await _mediator.Send(new CancelEventCommand(id, CurrentUserId!.Value));
            return result.IsSuccess ? NoContent() : BadRequest(result.Error);
        }

        // ── Join / Leave ──────────────────────────────────────────────────────────

        /// <summary>
        /// Join an event. Participant count increases immediately.
        /// </summary>
        [HttpPost("{id:guid}/join")]
        [Authorize]
        public async Task<IActionResult> Join(Guid id)
        {
            var result = await _mediator.Send(new JoinEventCommand(id, CurrentUserId!.Value));
            return result.IsSuccess ? Ok(new { message = "Joined successfully." }) : BadRequest(result.Error);
        }

        /// <summary>
        /// Leave an event.
        /// </summary>
        [HttpPost("{id:guid}/leave")]
        [Authorize]
        public async Task<IActionResult> Leave(Guid id)
        {
            var result = await _mediator.Send(new LeaveEventCommand(id, CurrentUserId!.Value));
            return result.IsSuccess ? Ok(new { message = "Left successfully." }) : BadRequest(result.Error);
        }
    }


  
}