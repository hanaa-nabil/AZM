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
        public async Task<IActionResult> AddPhone([FromBody] AddPhoneRequestDto dto)
        {
            var result = await _mediator.Send(new AddPhoneCommand(dto));
            return result.IsSuccess
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Step 4 — Complete profile: select favourite sports (1 or more) and optionally upload a photo.
        /// Final step — issues JWT token upon success.
        /// Applies to both email and Google registered users.
        /// </summary>
        [HttpPost("complete-profile")]
        public async Task<IActionResult> CompleteProfile([FromBody] CompleteProfileRequestDto dto)
        {
            var result = await _mediator.Send(new CompleteProfileCommand(dto));
            return result.IsSuccess
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Sign in with email and password.
        /// Returns a JWT token on success.
        /// </summary>
        [HttpPost("login")]
        [EnableRateLimiting("registration")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            var result = await _mediator.Send(new LoginCommand(dto));
            return result.IsSuccess
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Request a password-reset OTP. Always returns 200 to avoid leaking account existence.
        /// </summary>
        [HttpPost("forgot-password")]
        [EnableRateLimiting("registration")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto dto)
        {
            var result = await _mediator.Send(new ForgotPasswordCommand(dto));
            return result.IsSuccess
                ? Ok(new { message = "If an account with that email exists, a reset code has been sent." })
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Reset password using the OTP sent to the user's email.
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto dto)
        {
            var result = await _mediator.Send(new ResetPasswordCommand(dto));
            return result.IsSuccess
                ? Ok(new { message = "Password reset successfully. You may now sign in." })
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Sign in with Google.
        /// Returns JWT if existing account.
        /// Returns userId + email (no token) if new account — proceed to complete-registration.
        /// </summary>
        [HttpPost("google")]
        [EnableRateLimiting("registration")]
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
        public async Task<IActionResult> CompleteRegistration([FromBody] SocialCompleteRegistrationDto dto)
        {
            var result = await _mediator.Send(new SocialCompleteRegistrationCommand(dto));
            return result.IsSuccess
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

        /// <summary>
        /// Step 3b — Verify phone number using the Firebase ID token.
        /// Must be called after add-phone (email flow) or complete-registration (Google flow).
        /// Unlocks complete-profile.
        /// </summary>
        [HttpPost("verify-phone")]
        public async Task<IActionResult> VerifyPhone([FromBody] VerifyPhoneRequestDto dto)
        {
            var result = await _mediator.Send(new VerifyPhoneCommand(dto));
            return result.IsSuccess
                ? Ok(new { message = "Phone number verified successfully." })
                : StatusCode(result.StatusCode, new { error = result.Error });
        }

    }
}