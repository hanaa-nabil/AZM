using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class VerifyPhoneRequestDto
    {
        [Required]
        [DefaultValue("3fa85f64-5717-4562-b3fc-2c963f66afa6")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [DefaultValue("eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJmaXJlYmFzZSIsInN1YiI6InVzZXJfaWRfaGVyZSJ9.SIGNATURE")]
        public string FirebaseIdToken { get; set; } = string.Empty;
    }
}