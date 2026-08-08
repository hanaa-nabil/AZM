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
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public bool IsIdVerified { get; set; }
        public List<Sport> Sports { get; set; } = new();
        public string? Location { get; set; }
        public int EventsJoinedCount { get; set; }
        public int EventsCompletedCount { get; set; }
        public double TotalDistanceMeters { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string? FcmToken { get; set; }
    }
}
