using System.ComponentModel.DataAnnotations;

namespace AZM.Application.DTOs
{
    public class AddPhoneRequestDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        [RegularExpression(@"^\+?[1-9]\d{6,14}$",
            ErrorMessage = "Phone number must be in valid international format (e.g. +201012345678).")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}