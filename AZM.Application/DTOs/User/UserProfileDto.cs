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
        public int TotalStepCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public bool? IsFollowedByMe { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public string? FcmToken { get; set; }
        public double? Latitude { get; private set; }
        public double? Longitude { get; private set; }
        public DateTime? LocationUpdatedAt { get; private set; }
        public bool ShareLocation { get; private set; }

        public void SetLocationSharing(bool enabled)
        {
            ShareLocation = enabled;
            if (!enabled) { Latitude = null; Longitude = null; LocationUpdatedAt = null; }
        }

        public void UpdateLocation(double lat, double lng)
        {
            Latitude = lat; Longitude = lng; LocationUpdatedAt = DateTime.UtcNow;
        }
    }
}
