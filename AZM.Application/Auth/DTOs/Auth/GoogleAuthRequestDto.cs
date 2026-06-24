using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class GoogleAuthRequestDto
    {
        [Required]
        public string IdToken { get; set; } = string.Empty;
    }
}