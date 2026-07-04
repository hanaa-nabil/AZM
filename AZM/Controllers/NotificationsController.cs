using AZM.Application.Notifications.Commands;
using AZM.Application.Notifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AZM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = Guid.Parse(User.FindFirst("sub")!.Value);
            var result = await _mediator.Send(new GetMyNotificationsQuery(userId, page, pageSize));
            return Ok(result);
        }

        [HttpPost("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = Guid.Parse(User.FindFirst("sub")!.Value);
            await _mediator.Send(new MarkNotificationReadCommand(id, userId));
            return NoContent();
        }
    }
}
