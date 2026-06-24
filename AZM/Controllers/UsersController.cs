using AZM.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AZM.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepo;

        public UsersController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
        }

        // PUT /api/users/me/fcm-token
        [HttpPut("me/fcm-token")]
        public async Task<IActionResult> UpdateFcmToken([FromBody] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return BadRequest("FCM token cannot be empty");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Unauthorized();

            await _userRepo.UpdateFcmTokenAsync(userId, token);

            return Ok(new { message = "FCM token updated" });
        }
    }
}
