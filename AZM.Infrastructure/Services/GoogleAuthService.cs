using AZM.Domain.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;

namespace AZM.Infrastructure.Services
{
    public class GoogleAuthService : ISocialAuthService
    {
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(ILogger<GoogleAuthService> logger)
        {
            _logger = logger;

            if (FirebaseApp.DefaultInstance is null)
            {
                _logger.LogWarning("FirebaseApp.DefaultInstance was null — attempting to initialize via Application Default Credentials.");
                try
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.GetApplicationDefault()
                    });
                    _logger.LogInformation("FirebaseApp initialized successfully via ADC.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to initialize FirebaseApp via ADC. Google/Firebase sign-in will not work until this is resolved.");
                }
            }
        }

        public async Task<SocialUserInfo?> VerifyGoogleTokenAsync(string idToken)
        {
            if (string.IsNullOrWhiteSpace(idToken))
                return null;

            if (FirebaseApp.DefaultInstance is null)
            {
                _logger.LogError("Cannot verify Google token — FirebaseApp is not initialized.");
                return null;
            }

            try
            {
                var decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                var claims = decoded.Claims;

                var email = claims.TryGetValue("email", out var e) ? e?.ToString() : null;
                var name = claims.TryGetValue("name", out var n) ? n?.ToString() : null;

                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogWarning("Firebase token has no email claim. Uid: {Uid}", decoded.Uid);
                    return null;
                }

                var parts = (name ?? "").Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                return new SocialUserInfo
                {
                    SocialId = decoded.Uid,
                    Email = email,
                    FirstName = parts.Length > 0 ? parts[0] : "",
                    LastName = parts.Length > 1 ? parts[1] : ""
                };
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogWarning(ex, "Firebase token validation failed: {ErrorCode} — {Message}", ex.AuthErrorCode, ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error verifying Firebase id token via FirebaseAdmin.Auth.");
                return null;
            }
        }
    }
}