using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class VerifyOtpRequestDto
    {
        [Required, EmailAddress, MaxLength(256)]
        [DefaultValue("john.doe@example.com")]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be exactly 6 digits.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must contain digits only.")]
        [DefaultValue("123456")]
        public string Code { get; set; } = string.Empty;
    }
}