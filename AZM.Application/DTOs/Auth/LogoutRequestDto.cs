using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.Auth
{
    public class LogoutRequestDto
    {
        public string FcmToken { get; set; } = string.Empty;
    }
}
