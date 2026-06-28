using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required, EmailAddress, MaxLength(256)]
        [DefaultValue("john.doe@example.com")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DefaultValue("Test@1234")]
        public string Password { get; set; } = string.Empty;
    }
}