using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.Auth
{
    public class VerifyOtpResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
        public string? TokenType { get; set; } = "Bearer";
    }
}
