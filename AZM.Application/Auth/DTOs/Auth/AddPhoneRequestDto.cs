using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class AddPhoneRequestDto
    {
        [Required]
        [DefaultValue("3fa85f64-5717-4562-b3fc-2c963f66afa6")]
        public string UserId { get; set; } = string.Empty;

        [Required, Phone]
        [DefaultValue("+201012345678")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}