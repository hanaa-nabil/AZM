using AZM.Domain.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace AZM.Infrastructure.Services
{
    public class GoogleAuthService : ISocialAuthService
    {
        private readonly string _clientId;

        public GoogleAuthService(IConfiguration configuration)
        {
            _clientId = configuration["Google:ClientId"]
                ?? throw new InvalidOperationException("Google ClientId is not configured.");
        }

        public async Task<SocialUserInfo?> VerifyGoogleTokenAsync(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                return new SocialUserInfo
                {
                    SocialId = payload.Subject,
                    Email = payload.Email,
                    FirstName = payload.GivenName ?? string.Empty,
                    LastName = payload.FamilyName ?? string.Empty
                };
            }
            catch
            {
                // Token is invalid, expired, or tampered with
                return null;
            }
        }
    }
}