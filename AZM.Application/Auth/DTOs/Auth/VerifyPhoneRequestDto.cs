using System.ComponentModel.DataAnnotations;

namespace AZM.Application.Auth.DTOs.Auth
{
    public class VerifyPhoneRequestDto
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// The Firebase ID token returned to the frontend after
        /// the user successfully enters the SMS OTP in the app.
        /// </summary>
        [Required]
        public string FirebaseIdToken { get; set; } = string.Empty;
    }
}