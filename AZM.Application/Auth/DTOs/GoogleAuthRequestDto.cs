using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs
{
    public class GoogleAuthRequestDto
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}