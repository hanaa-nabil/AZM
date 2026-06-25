using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs
{
    public class AddPhoneRequestDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required, Phone]
        [MinLength(7, ErrorMessage = "Phone number is too short.")]
        [MaxLength(15, ErrorMessage = "Phone number is too long.")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}