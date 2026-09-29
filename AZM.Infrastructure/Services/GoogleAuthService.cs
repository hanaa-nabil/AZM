using AZM.Domain.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.Extensions.Logging;

namespace AZM.Infrastructure.Services
{
    public class GoogleAuthService : ISocialAuthService
    {
        private readonly ILogger<GoogleAuthService> _logger;

        public GoogleAuthService(ILogger<GoogleAuthService> logger)
        {
            _logger = logger;
        }

        public async Task<(SocialUserInfo? User, string? Error)> VerifyGoogleTokenAsync(string idToken)
        {
            if (string.IsNullOrWhiteSpace(idToken))
                return (null, "No token provided.");

            if (FirebaseApp.DefaultInstance is null)
            {
                _logger.LogError("Cannot verify Google token — FirebaseApp is not initialized.");
                return (null, "Firebase is not initialized on the server.");
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
                    return (null, "Google token has no email claim.");
                }

                var parts = (name ?? "").Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
                var user = new SocialUserInfo
                {
                    SocialId = decoded.Uid,
                    Email = email,
                    FirstName = parts.Length > 0 ? parts[0] : "",
                    LastName = parts.Length > 1 ? parts[1] : ""
                };
                return (user, null);
            }
            catch (FirebaseAuthException ex)
            {
                _logger.LogWarning(ex, "Firebase token validation failed: {ErrorCode} — {Message}", ex.AuthErrorCode, ex.Message);
                return (null, $"Firebase token validation failed: {ex.AuthErrorCode} — {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error verifying Firebase id token via FirebaseAdmin.Auth.");
                return (null, $"Unexpected error verifying token: {ex.Message}");
            }
        }
    }
}