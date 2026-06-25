using AZM.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs
{
    public class CompleteProfileRequestDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// At least one sport must be selected.
        /// Valid values: Running, Cycling, Skating, Other
        /// </summary>
        [Required, MinLength(1, ErrorMessage = "Please select at least one sport.")]
        public List<Sport> Sports { get; set; } = new();

        /// <summary>
        /// Optional. Base64-encoded image string.
        /// If null or empty, no photo is uploaded and ProfilePhotoUrl stays null.
        /// The frontend should show the user's initials as fallback.
        /// </summary>
        public string? PhotoBase64 { get; set; }
    }
}