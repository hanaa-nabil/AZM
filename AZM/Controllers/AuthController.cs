using AZM.Application.Auth.Commands;
using AZM.Application.Auth.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AZM.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Step 1 — Register with name, email, password, gender, birthdate.
        /// Returns userId + email. A 6-digit OTP is sent to the email.
        /// </summary>
        [HttpPost("register")]
        [EnableRateLimiting("registration")]
        [ProducesResponseType(typeof(RegisterResponseDto), 201)]
        [ProducesResponseType(400), ProducesResponseType(409), ProducesResponseType(429)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var result = await _mediator.Send(new RegisterCommand(dto));
            return result.IsSuccess
                ? StatusCode(201, result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Step 2 — Verify the email OTP.
        /// </summary>
        [HttpPost("verify-otp")]
        [ProducesResponseType(200), ProducesResponseType(400)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto dto)
        {
            var result = await _mediator.Send(new VerifyOtpCommand(dto));
            return result.IsSuccess
                ? Ok(new { message = "Email verified successfully." })
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Resend OTP to the registered email. 60-second cooldown.
        /// </summary>
        [HttpPost("resend-otp")]
        [EnableRateLimiting("registration")]
        [ProducesResponseType(200), ProducesResponseType(400), ProducesResponseType(429)]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequestDto dto)
        {
            var result = await _mediator.Send(new ResendOtpCommand(dto));
            return result.IsSuccess
                ? Ok(new { message = "OTP resent successfully." })
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Step 3 — Add phone number after email is verified.
        /// Returns userId + email. No token yet.
        /// </summary>
        [HttpPost("add-phone")]
        [ProducesResponseType(typeof(RegisterResponseDto), 200)]
        [ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(409)]
        public async Task<IActionResult> AddPhone([FromBody] AddPhoneRequestDto dto)
        {
            var result = await _mediator.Send(new AddPhoneCommand(dto));
            return result.IsSuccess
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Step 4 — Complete profile: select favourite sports (1 or more) and optionally upload a photo.
        /// This is the final step. Issues the JWT token upon success.
        /// Applies to both email-registered and Google-registered users.
        /// </summary>
        [HttpPost("complete-profile")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(400), ProducesResponseType(404)]
        public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequestDto dto)
        {
            var result = await _mediator.Send(new CompleteProfileCommand(dto));
            return result.IsSuccess
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Sign in with Google. Returns userId + email if new account (IsPendingPhoneNumber flow).
        /// Returns JWT directly if existing account.
        /// </summary>
        [HttpPost("google")]
        [EnableRateLimiting("registration")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(400), ProducesResponseType(401), ProducesResponseType(409), ProducesResponseType(429)]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleAuthRequestDto dto)
        {
            var result = await _mediator.Send(new GoogleSignInCommand(dto));
            return result.IsSuccess
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Google Step 2 — Add phone number for new Google accounts.
        /// Returns userId + email. Proceed to complete-profile next.
        /// </summary>
        [HttpPost("complete-registration")]
        [ProducesResponseType(typeof(RegisterResponseDto), 200)]
        [ProducesResponseType(400), ProducesResponseType(404), ProducesResponseType(409)]
        public async Task<IActionResult> CompleteRegistration([FromBody] SocialCompleteRegistrationDto dto)
        {
            var result = await _mediator.Send(new SocialCompleteRegistrationCommand(dto));
            return result.IsSuccess
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }
    }
}