using AZM.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace AZM.Domain.Entities
{
    public class User : IdentityUser<Guid>
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

        public string? GoogleId { get; set; }
        public bool IsGoogleAccount { get; set; } = false;
        public bool IsPendingPhoneNumber { get; set; } = false;

        // Navigation properties
        public UserProfile? Profile { get; set; }
        public ICollection<EventParticipant> EventParticipants { get; set; } = new List<EventParticipant>();
        public ICollection<Achievement> Achievements { get; set; } = new List<Achievement>();

<<<<<<< HEAD
=======
        public string? FcmToken { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public ICollection<UserSport> Sports { get; set; } = new List<UserSport>();

>>>>>>> DB Back to local, Auth working technically
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}