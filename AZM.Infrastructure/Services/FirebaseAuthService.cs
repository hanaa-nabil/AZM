using AZM.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;

namespace AZM.Infrastructure.Services
{
    public class FirebaseAuthService : IFirebaseAuthService
    {
        private readonly string _projectId;
        private readonly HttpClient _httpClient;

        private const string PublicKeysUrl =
            "https://www.googleapis.com/robot/v1/metadata/x509/securetoken@system.gserviceaccount.com";

        public FirebaseAuthService(IConfiguration configuration, HttpClient httpClient)
        {
            _projectId = configuration["Firebase:ProjectId"]
                ?? throw new InvalidOperationException("Firebase:ProjectId is not configured.");
            _httpClient = httpClient;
        }

        public async Task<string?> VerifyPhoneTokenAsync(string firebaseIdToken)
        {
            try
            {
                // 1. Fetch Firebase public keys (Google rotates these periodically)
                var keys = await _httpClient.GetFromJsonAsync<Dictionary<string, string>>(PublicKeysUrl);
                if (keys is null) return null;

                var signingKeys = keys.Values.Select(cert =>
                {
                    var x509 = new System.Security.Cryptography.X509Certificates.X509Certificate2(
                        System.Text.Encoding.ASCII.GetBytes(cert));
                    return new X509SecurityKey(x509);
                }).ToList();

                // 2. Validate the token
                var handler = new JwtSecurityTokenHandler();
                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = $"https://securetoken.google.com/{_projectId}",
                    ValidateAudience = true,
                    ValidAudience = _projectId,
                    ValidateLifetime = true,
                    IssuerSigningKeys = signingKeys,
                    ValidateIssuerSigningKey = true,
                };

                var principal = handler.ValidateToken(
                    firebaseIdToken, validationParams, out _);

                // 3. Extract phone number from the token claims
                var phone = principal.FindFirst("phone_number")?.Value;
                return string.IsNullOrEmpty(phone) ? null : phone;
            }
            catch
            {
                return null;
            }
        }
    }
}