using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class ResetPasswordRequestDto
    {
        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        /// <summary>The 6-digit OTP sent to the user's email.</summary>
        [Required]
        public string Otp { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(128)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\#_])[A-Za-z\d@$!%*?&\#_]{8,}$",
            ErrorMessage = "Password must have uppercase, lowercase, digit, and special character.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}