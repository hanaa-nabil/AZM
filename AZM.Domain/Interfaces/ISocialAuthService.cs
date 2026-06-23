namespace AZM.Domain.Interfaces
{
    public class SocialUserInfo
    {
        public string SocialId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public interface ISocialAuthService
    {
        Task<SocialUserInfo?> VerifyGoogleTokenAsync(string idToken);
    }
}