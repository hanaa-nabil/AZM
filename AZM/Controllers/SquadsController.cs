using AZM.Application.Squad.Commands;
using AZM.Application.Squads.Commands;
using AZM.Application.Squads.Queries;
using AZM.Application.Squads.Requests;
using AZM.Infrastructure.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;

namespace AZM.Api.Controllers
{
    [Route("api/squads")]
    [ApiController]
    public class SquadsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<SquadChatHub> _hub;
        public SquadsController(IMediator mediator, IHubContext<SquadChatHub> hub)
        {
            _mediator = mediator;
            _hub = hub;
        }

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

            return result.IsSuccess ? Ok(new { id = result.Data }) : StatusCode(result.StatusCode == 0 ? 400 : result.StatusCode, new { error = result.Error });

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
       
        public async Task<IActionResult> GetRoster(Guid id)
        {
            var result = await _mediator.Send(new GetSquadRosterQuery(id));
            return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode == 0 ? 404 : result.StatusCode, new { error = result.Error });

        }

        

        [HttpPost("{id:guid}/join")]
        [Authorize]
        public async Task<IActionResult> Join(Guid id)
        {
            var result = await _mediator.Send(new RequestToJoinSquadCommand(id, CurrentUserId));
            return result.IsSuccess
                ? Ok(new { status = result.Data, message = result.Data == "Joined" ? "You joined the squad." : "Request sent." })
                : StatusCode(result.StatusCode == 0 ? 400 : result.StatusCode, new { error = result.Error });
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
        [HttpPost("{id:guid}/messages")]
        [Authorize]
        public async Task<IActionResult> SendMessage(Guid id, [FromBody] SendMessageRequest request)
        {
            var result = await _mediator.Send(new SendSquadMessageCommand(id, CurrentUserId, request.Content));

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode == 0 ? 400 : result.StatusCode, new { error = result.Error });

            // Deliver live to everyone currently in the squad's chat room
            await _hub.Clients.Group(id.ToString()).SendAsync("ReceiveMessage", result.Data);

            return Ok(result.Data);
        }
        [HttpGet("{id:guid}/messages")]
        [Authorize]
        public async Task<IActionResult> GetMessages(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var result = await _mediator.Send(new GetSquadMessagesQuery(id, CurrentUserId, page, pageSize));
            return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, result.Error);
        }


        [HttpPost("{id:guid}/read")]
        [Authorize]
        public async Task<IActionResult> MarkRead(Guid id)
        {
            var result = await _mediator.Send(new MarkSquadReadCommand(id, CurrentUserId));
            return result.IsSuccess
                ? NoContent()
                : StatusCode(result.StatusCode == 0 ? 400 : result.StatusCode, new { error = result.Error });
        }

        [HttpPut("{id:guid}/pin")]
        [Authorize]
        public async Task<IActionResult> PinEvent(Guid id, [FromBody] PinEventRequest request)
        {
            var result = await _mediator.Send(new PinSquadEventCommand(id, CurrentUserId, request.EventId));
            return result.IsSuccess
                ? NoContent()
                : StatusCode(result.StatusCode == 0 ? 400 : result.StatusCode, new { error = result.Error });
        }

        [HttpDelete("messages/{messageId:guid}")]
        [Authorize]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            var result = await _mediator.Send(new DeleteSquadMessageCommand(messageId, CurrentUserId));
            return result.IsSuccess
                ? NoContent()
                : StatusCode(result.StatusCode == 0 ? 400 : result.StatusCode, new { error = result.Error });
        }

        public class PinEventRequest { public Guid? EventId { get; set; } }   // null unpins
    }
}
