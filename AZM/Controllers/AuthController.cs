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
        /// Register a new account. Returns a JWT and sends a 6-digit OTP to the email.
        /// </summary>
        [HttpPost("register")]
        [EnableRateLimiting("registration")]
        [ProducesResponseType(typeof(AuthResponseDto), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(429)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            var result = await _mediator.Send(new RegisterCommand(dto));

            return result.IsSuccess
                ? StatusCode(201, result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Verify email using the 6-digit OTP sent during registration.
        /// </summary>
        [HttpPost("verify-otp")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequestDto dto)
        {
            var result = await _mediator.Send(new VerifyOtpCommand(dto));

            return result.IsSuccess
                ? Ok(new { message = "Email verified successfully." })
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Resend OTP to the registered email. Has a 60-second cooldown.
        /// </summary>
        [HttpPost("resend-otp")]
        [EnableRateLimiting("registration")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(429)]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequestDto dto)
        {
            var result = await _mediator.Send(new ResendOtpCommand(dto));

            return result.IsSuccess
                ? Ok(new { message = "OTP resent successfully." })
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Sign in with Google. Returns a JWT.
        /// If the account is new, IsPendingPhoneNumber will be true — call /complete-registration next.
        /// </summary>
        [HttpPost("google")]
        [EnableRateLimiting("registration")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(409)]
        [ProducesResponseType(429)]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleAuthRequestDto dto)
        {
            var result = await _mediator.Send(new GoogleSignInCommand(dto));

            return result.IsSuccess
                ? StatusCode(result.StatusCode, result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Complete registration for Google accounts by providing a phone number.
        /// </summary>
        [HttpPost("complete-registration")]
        [ProducesResponseType(typeof(AuthResponseDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> CompleteRegistration([FromBody] SocialCompleteRegistrationDto dto)
        {
            var result = await _mediator.Send(new SocialCompleteRegistrationCommand(dto));

            return result.IsSuccess
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }
    }
}