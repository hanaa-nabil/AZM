using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class GoogleAuthRequestDto
    {
        [Required]
        [DefaultValue("eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJhY2NvdW50cy5nb29nbGUuY29tIiwic3ViIjoiMTIzNDU2Nzg5MCIsImVtYWlsIjoiam9obi5kb2VAZ21haWwuY29tIn0.SIGNATURE")]
        public string IdToken { get; set; } = string.Empty;
    }
}