using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class LoginRequestDto
    {
        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}