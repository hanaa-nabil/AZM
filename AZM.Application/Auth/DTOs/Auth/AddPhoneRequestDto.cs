using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class AddPhoneRequestDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}