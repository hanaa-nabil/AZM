namespace AZM.Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpEmailAsync(string toEmail, string firstName, string otp);
        Task SendWelcomeEmailAsync(string toEmail, string firstName);
    }
}