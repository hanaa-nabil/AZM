using AZM.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required, MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s\-']{1,50}$",
            ErrorMessage = "First name contains invalid characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s\-']{1,50}$",
            ErrorMessage = "Last name contains invalid characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(128)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\#_])[A-Za-z\d@$!%*?&\#_]{8,}$",
            ErrorMessage = "Password must have uppercase, lowercase, digit, and special character.")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public Gender Gender { get; set; }
    }
}