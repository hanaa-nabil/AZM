using AZM.Domain.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace AZM.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _config["Email:SenderName"],
                _config["Email:SenderEmail"]));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _config["Email:SmtpHost"],
                int.Parse(_config["Email:SmtpPort"]!),
                SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(
                _config["Email:Username"],
                _config["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }

        public async Task SendOtpEmailAsync(string toEmail, string firstName, string otp)
        {
            var formattedOtp = string.Join(" ", otp.ToCharArray());
            var subject = "Your AZM verification code";
            var body = $@"
<!DOCTYPE html><html><body style='margin:0;padding:0;'>
<table width='100%' cellpadding='0' cellspacing='0' 
       style='background:#080f18;min-height:100%;'>
  <tr><td align='center' style='padding:32px 16px;background:#080f18;'>
<table width='560' cellpadding='0' cellspacing='0' style='background:#0A1220;border-radius:10px;border:1px solid #1a2d3e;overflow:hidden;'>

  <tr><td style='background:#0d1e2c;padding:20px 32px;border-bottom:1px solid #1a2d3e;'>
    <span style='font-size:22px;font-weight:800;color:#00D4C8;letter-spacing:1px;'>AZM</span>
  </td></tr>

  <tr><td style='padding:36px 32px;'>
    <h1 style='color:#e2eaf2;font-size:22px;font-weight:700;margin:0 0 8px;'>Verify your email</h1>
    <p style='color:#637a90;font-size:14px;margin:0 0 28px;line-height:1.5;'>Hi {firstName}, use the code below to confirm your AZM account.</p>

    <table width='100%' cellpadding='0' cellspacing='0' style='background:#0d1e2c;border:1px solid #1a3548;border-radius:10px;'>
      <tr><td style='padding:24px;text-align:center;'>
        <p style='color:#637a90;font-size:11px;text-transform:uppercase;letter-spacing:1.5px;margin:0 0 14px;'>Your verification code</p>
        <p style='color:#00D4C8;font-size:38px;font-weight:800;letter-spacing:14px;font-family:Courier New,monospace;margin:0;padding-left:14px;'>{formattedOtp}</p>
        <p style='color:#4a6070;font-size:12px;margin:12px 0 0;'>Expires in 10 minutes</p>
      </td></tr>
    </table>

    <table width='100%' cellpadding='0' cellspacing='0' style='margin:20px 0;'>
      <tr><td style='background:rgba(0,212,200,0.07);border-left:3px solid #00D4C8;border-radius:0 6px 6px 0;padding:12px 16px;'>
        <p style='color:#637a90;font-size:13px;margin:0;line-height:1.5;'>Enter this code in the AZM app to confirm your email and continue setting up your profile.</p>
      </td></tr>
    </table>

    <p style='color:#3a5060;font-size:12px;text-align:center;margin:20px 0 0;'>Didn't request this? You can safely ignore this email.</p>
  </td></tr>

  <tr><td style='background:#080f18;padding:16px 32px;border-top:1px solid #1a2d3e;text-align:center;'>
    <p style='color:#3a5060;font-size:11px;margin:0;'>AZM &middot; The athlete&apos;s platform</p>
  </td></tr>

</table></td></tr></table>
</body></html>";
            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string firstName)
        {
            var subject = "Welcome to AZM!";
            var body = $@"
<!DOCTYPE html><html><body style='margin:0;padding:0;'>
<table width='100%' cellpadding='0' cellspacing='0' 
       style='background:#080f18;min-height:100%;'>
  <tr><td align='center' style='padding:32px 16px;background:#080f18;'>
<table width='560' cellpadding='0' cellspacing='0' style='background:#0A1220;border-radius:10px;border:1px solid #1a2d3e;overflow:hidden;'>

  <tr><td style='background:#0d1e2c;padding:20px 32px;border-bottom:1px solid #1a2d3e;'>
    <span style='font-size:22px;font-weight:800;color:#00D4C8;letter-spacing:1px;'>AZM</span>
  </td></tr>

  <tr><td style='padding:36px 32px;'>
    <table cellpadding='0' cellspacing='0' style='margin-bottom:8px;'><tr>
      <td style='background:rgba(0,212,200,0.12);border:1px solid rgba(0,212,200,0.25);border-radius:20px;padding:5px 14px;font-size:12px;color:#00D4C8;font-weight:600;letter-spacing:0.5px;'>&#10022; Account verified</td>
    </tr></table>

    <h1 style='color:#e2eaf2;font-size:22px;font-weight:700;margin:12px 0 8px;'>Welcome to the squad, {firstName}!</h1>
    <p style='color:#637a90;font-size:14px;margin:0 0 28px;line-height:1.5;'>Your account is active. Start discovering events and athletes near you.</p>

    <table width='100%' cellpadding='0' cellspacing='0'><tr>
      <td width='33%' style='padding:4px;'><table width='100%' style='background:#0d1e2c;border:1px solid #1a3548;border-radius:8px;'><tr><td style='padding:14px 10px;text-align:center;'><div style='font-size:22px;margin-bottom:6px;'>🏃</div><div style='color:#637a90;font-size:11px;'>Find events</div></td></tr></table></td>
      <td width='33%' style='padding:4px;'><table width='100%' style='background:#0d1e2c;border:1px solid #1a3548;border-radius:8px;'><tr><td style='padding:14px 10px;text-align:center;'><div style='font-size:22px;margin-bottom:6px;'>🤝</div><div style='color:#637a90;font-size:11px;'>Join teams</div></td></tr></table></td>
      <td width='33%' style='padding:4px;'><table width='100%' style='background:#0d1e2c;border:1px solid #1a3548;border-radius:8px;'><tr><td style='padding:14px 10px;text-align:center;'><div style='font-size:22px;margin-bottom:6px;'>📍</div><div style='color:#637a90;font-size:11px;'>Near you</div></td></tr></table></td>
    </tr></table>

    <table cellpadding='0' cellspacing='0' style='margin:28px auto 0;'><tr>
      <td style='background:#00D4C8;border-radius:50px;padding:14px 36px;'>
        <a href='#' style='color:#0A1220;font-weight:700;font-size:15px;text-decoration:none;'>Open AZM →</a>
      </td>
    </tr></table>
  </td></tr>

  <tr><td style='background:#080f18;padding:16px 32px;border-top:1px solid #1a2d3e;text-align:center;'>
    <p style='color:#3a5060;font-size:11px;margin:0;'>AZM &middot; The athlete&apos;s platform</p>
  </td></tr>

</table></td></tr></table>
</body></html>";
            await SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPasswordResetOtpAsync(string toEmail, string firstName, string otp)
        {
            var formattedOtp = string.Join(" ", otp.ToCharArray());
            var subject = "Reset your AZM password";
            var body = $@"
<!DOCTYPE html><html><body style='margin:0;padding:0;'>
<table width='100%' cellpadding='0' cellspacing='0' 
       style='background:#080f18;min-height:100%;'>
  <tr><td align='center' style='padding:32px 16px;background:#080f18;'>
<table width='560' cellpadding='0' cellspacing='0' style='background:#0A1220;border-radius:10px;border:1px solid #1a2d3e;overflow:hidden;'>

  <tr><td style='background:#0d1e2c;padding:20px 32px;border-bottom:1px solid #1a2d3e;'>
    <span style='font-size:22px;font-weight:800;color:#00D4C8;letter-spacing:1px;'>AZM</span>
  </td></tr>

  <tr><td style='padding:36px 32px;'>
    <h1 style='color:#e2eaf2;font-size:22px;font-weight:700;margin:0 0 8px;'>Reset your password</h1>
    <p style='color:#637a90;font-size:14px;margin:0 0 28px;line-height:1.5;'>Hi {firstName}, we received a request to reset your AZM password.</p>

    <table width='100%' cellpadding='0' cellspacing='0' style='background:#0d1e2c;border:1px solid #1a3548;border-radius:10px;'>
      <tr><td style='padding:24px;text-align:center;'>
        <p style='color:#637a90;font-size:11px;text-transform:uppercase;letter-spacing:1.5px;margin:0 0 14px;'>Password reset code</p>
        <p style='color:#00D4C8;font-size:38px;font-weight:800;letter-spacing:14px;font-family:Courier New,monospace;margin:0;padding-left:14px;'>{formattedOtp}</p>
        <p style='color:#4a6070;font-size:12px;margin:12px 0 0;'>Expires in 10 minutes</p>
      </td></tr>
    </table>

    <table width='100%' cellpadding='0' cellspacing='0' style='margin:20px 0;'>
      <tr><td style='background:rgba(0,212,200,0.07);border-left:3px solid #00D4C8;border-radius:0 6px 6px 0;padding:12px 16px;'>
        <p style='color:#637a90;font-size:13px;margin:0;line-height:1.5;'>Enter this code in the AZM app on the password reset screen.</p>
      </td></tr>
    </table>

    <table width='100%' cellpadding='0' cellspacing='0' style='background:rgba(255,100,50,0.07);border:1px solid rgba(255,100,50,0.2);border-radius:8px;'>
      <tr><td style='padding:14px;'>
        <p style='color:#8a6050;font-size:12px;margin:0;line-height:1.5;'>⚠️ Didn't request this? Your account may be at risk — consider changing your password immediately.</p>
      </td></tr>
    </table>
  </td></tr>

  <tr><td style='background:#080f18;padding:16px 32px;border-top:1px solid #1a2d3e;text-align:center;'>
    <p style='color:#3a5060;font-size:11px;margin:0;'>AZM &middot; The athlete&apos;s platform</p>
  </td></tr>

</table></td></tr></table>
</body></html>";
            await SendEmailAsync(toEmail, subject, body);
        }
    }
}