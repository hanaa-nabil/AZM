using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class ResetPasswordRequestDto
    {
        [Required, EmailAddress, MaxLength(256)]
        [DefaultValue("john.doe@example.com")]
        public string Email { get; set; } = string.Empty;

        /// <summary>The 6-digit OTP sent to the user's email.</summary>
        [Required]
        [DefaultValue("123456")]
        public string Otp { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(128)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\#_])[A-Za-z\d@$!%*?&\#_]{8,}$",
            ErrorMessage = "Password must have uppercase, lowercase, digit, and special character.")]
        [DefaultValue("NewPass@1234")]
        public string NewPassword { get; set; } = string.Empty;
    }
}