using AZM.Domain.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class RegisterRequestDto
    {
        [Required, MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s\-']{1,50}$",
           ErrorMessage = "First name contains invalid characters.")]
        [DefaultValue("John")]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z\s\-']{1,50}$",
            ErrorMessage = "Last name contains invalid characters.")]
        [DefaultValue("Doe")]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(256)]
        [DefaultValue("john.doe@example.com")]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(8), MaxLength(128)]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&\#_])[A-Za-z\d@$!%*?&\#_]{8,}$",
            ErrorMessage = "Password must have uppercase, lowercase, digit, and special character.")]
        [DefaultValue("Test@1234")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DefaultValue("1990-01-15")]
        public DateTime BirthDate { get; set; }

        [Required]
        [DefaultValue(0)]
        public Gender Gender { get; set; }
    }
}