using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class SocialCompleteRegistrationDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}