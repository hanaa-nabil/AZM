using AZM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.User
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public bool IsIdVerified { get; set; }
        public List<Sport> Sports { get; set; } = new();
        public string? Location { get; set; }
        public int EventsJoinedCount { get; set; }
        public int EventsCompletedCount { get; set; }
        public double TotalDistanceMeters { get; set; }
    }
}
