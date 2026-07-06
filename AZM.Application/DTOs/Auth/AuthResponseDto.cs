namespace AZM.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Token { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public bool IsRegistrationComplete { get; set; } = true;
        public bool RequiresPhone { get; set; } = false;
        public DateTime? ExpiresAtUtc { get; set; }
        public string? TokenType { get; set; } = "Bearer";
    }
}