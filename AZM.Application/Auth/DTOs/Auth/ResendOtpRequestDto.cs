using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class ResendOtpRequestDto
    {
        [Required, EmailAddress, MaxLength(256)]
        [DefaultValue("john.doe@example.com")]
        public string Email { get; set; } = string.Empty;
    }
}