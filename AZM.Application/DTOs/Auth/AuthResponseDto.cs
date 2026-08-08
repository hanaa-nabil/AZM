namespace AZM.Application.DTOs.Auth
{
    public class AuthResponseDto
    {
        public Guid? UserId { get; set; }
        public string? Email { get; set; } 
        public string? FullName { get; set; } 
        public string? Token { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public bool? IsRegistrationComplete { get; set; } 
        public bool? RequiresPhone { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
        public string? TokenType { get; set; } 
        public string? Message { get; set; } 
        public bool? IsVerified { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiresAtUtc { get; set; }
    }
}