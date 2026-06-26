namespace AZM.Application.Auth.DTOs.Auth
{
    public class RegisterResponseDto
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the user must verify their email.
        /// </summary>
        public bool EmailVerificationRequired { get; set; }

        /// <summary>
        /// Indicates whether the user must add a phone number.
        /// </summary>
        public bool PhoneNumberRequired { get; set; }

        public string Message { get; set; } = string.Empty;
    }
}