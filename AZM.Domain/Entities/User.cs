using AZM.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? NationalityCode { get; set; }    
        public string? IdDocumentNumber { get; set; }      
        public string? IdDocumentImageUrl { get; set; }     
        public bool IsIdVerified { get; set; } = false;
        public string? FaceScanFrontUrl { get; set; }
        public string? FaceScanLeftUrl { get; set; }
        public string? FaceScanRightUrl { get; set; }
        public bool IsFaceVerified { get; set; } = false;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAtUtc { get; set; }
        public bool IsActive { get; set; } = true;

        // ----- Navigation properties -----
        public UserProfile? Profile { get; set; }
        public ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();
        public ICollection<LiveSession> LiveSessions { get; set; } = new List<LiveSession>();
        public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
