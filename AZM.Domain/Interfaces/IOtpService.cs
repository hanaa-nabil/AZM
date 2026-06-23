namespace AZM.Domain.Interfaces
{
    public interface IOtpService
    {
        Task<string> GenerateAndStoreOtpAsync(string email);
        Task<bool> ValidateOtpAsync(string email, string code);
        Task<bool> IsResendAllowedAsync(string email);
        Task InvalidateAllOtpsForEmailAsync(string email);
    }
}