using AZM.Domain.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class CompleteProfileRequestDto
    {
        [Required]
        [DefaultValue("3fa85f64-5717-4562-b3fc-2c963f66afa6")]
        public string UserId { get; set; } = string.Empty;

        [Required, MinLength(1, ErrorMessage = "Please select at least one sport.")]
        public List<Sport> Sports { get; set; } = new() { Sport.Running };

        public string? PhotoBase64 { get; set; } = null;
    }
}