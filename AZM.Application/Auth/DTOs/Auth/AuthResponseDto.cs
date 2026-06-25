namespace AZM.Application.Auth.DTOs.Auth
{
    public class AuthResponseDto
    {
        public Guid UserId { get; set; } 
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
        public string? ProfilePhotoUrl { get; set; }
    }
}