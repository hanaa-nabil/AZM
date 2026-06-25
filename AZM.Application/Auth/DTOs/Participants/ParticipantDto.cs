using AZM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Auth.DTOs.Participants
{
    public class ParticipantDto
    {
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public ParticipantStatus Status { get; set; }
        public DateTime JoinedAtUtc { get; set; }

    }
}
