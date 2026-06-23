using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using AZM.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AZM.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<OtpCode> _hasher;
        private const int OtpValidMinutes = 10;
        private const int ResendCooldownSeconds = 60;

        public OtpService(AppDbContext context, IPasswordHasher<OtpCode> hasher)
        {
            _context = context;
            _hasher = hasher;
        }

        public async Task<string> GenerateAndStoreOtpAsync(string email)
        {
            // Invalidate any previous unused OTPs for this email
            await InvalidateAllOtpsForEmailAsync(email);

            // Generate a cryptographically secure 6-digit code
            var code = RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            var otp = new OtpCode
            {
                Email = email.ToLowerInvariant(),
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(OtpValidMinutes)
            };

            // Hash the code before storing — never store OTPs in plaintext
            otp.Code = _hasher.HashPassword(otp, code);

            _context.OtpCodes.Add(otp);
            await _context.SaveChangesAsync();

            return code; // Return the raw code so we can email it
        }

        public async Task<bool> ValidateOtpAsync(string email, string code)
        {
            var otp = await _context.OtpCodes
                .Where(o => o.Email == email.ToLowerInvariant()
                         && !o.IsUsed
                         && o.ExpiresAtUtc > DateTime.UtcNow)
                .OrderByDescending(o => o.CreatedAtUtc)
                .FirstOrDefaultAsync();

            if (otp is null) return false;

            var result = _hasher.VerifyHashedPassword(otp, otp.Code, code);
            if (result == PasswordVerificationResult.Failed) return false;

            // Mark as used so it can't be replayed
            otp.IsUsed = true;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsResendAllowedAsync(string email)
        {
            // Find the most recent OTP for this email regardless of used/expired status
            var latest = await _context.OtpCodes
                .Where(o => o.Email == email.ToLowerInvariant())
                .OrderByDescending(o => o.CreatedAtUtc)
                .FirstOrDefaultAsync();

            if (latest is null) return true;

            // Allow resend only if 60 seconds have passed since the last one
            return (DateTime.UtcNow - latest.CreatedAtUtc).TotalSeconds >= ResendCooldownSeconds;
        }

        public async Task InvalidateAllOtpsForEmailAsync(string email)
        {
            var existing = await _context.OtpCodes
                .Where(o => o.Email == email.ToLowerInvariant() && !o.IsUsed)
                .ToListAsync();

            foreach (var otp in existing)
                otp.IsUsed = true;

            await _context.SaveChangesAsync();
        }
    }
}