namespace AZM.Domain.Entities
{
    // Stores the OTP we send to the user's email during registration.
    // One row per OTP attempt. Old ones are either expired or marked used.
    public class OtpCode
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;        // hashed before storing
        public DateTime ExpiresAtUtc { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}