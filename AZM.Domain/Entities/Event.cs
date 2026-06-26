using AZM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class Event
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public SportType SportType { get; private set; }
        public DifficultyLevel DifficultyLevel { get; private set; }
        public EventStatus Status { get; private set; }

        // Location (provided by Flutter as coordinates)
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string LocationName { get; private set; } = string.Empty;

        // Event timing
        public DateTime EventDate { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Organizer
        public Guid OrganizerId { get; private set; }
        public User Organizer { get; private set; } = null!;

        // Capacity
        public int MaxParticipants { get; private set; }

        // Route info (optional)
        public double? DistanceKm { get; private set; }
        public string? RouteImageUrl { get; private set; }
        public string? CoverImageUrl { get; private set; }

        // Navigation
        public ICollection<EventParticipant> Participants { get; private set; } = new List<EventParticipant>();

        // Computed
        public int ParticipantCount => Participants.Count(p => p.Status == ParticipantStatus.Joined);
        public bool IsFull => MaxParticipants > 0 && ParticipantCount >= MaxParticipants;

        private Event() { }

        public static Event Create(
            string title,
            string description,
            SportType sportType,
            DifficultyLevel difficultyLevel,
            double latitude,
            double longitude,
            string locationName,
            DateTime eventDate,
            Guid organizerId,
            int maxParticipants = 0,
            double? distanceKm = null,
            string? routeImageUrl = null,
            string? coverImageUrl = null)
        {
            return new Event
            {
                Id = Guid.NewGuid(),
                Title = title,
                Description = description,
                SportType = sportType,
                DifficultyLevel = difficultyLevel,
                Status = EventStatus.Upcoming,
                Latitude = latitude,
                Longitude = longitude,
                LocationName = locationName,
                EventDate = eventDate,
                OrganizerId = organizerId,
                MaxParticipants = maxParticipants,
                DistanceKm = distanceKm,
                RouteImageUrl = routeImageUrl,
                CoverImageUrl = coverImageUrl,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void Update(
            string title,
            string description,
            DifficultyLevel difficultyLevel,
            double latitude,
            double longitude,
            string locationName,
            DateTime eventDate,
            int maxParticipants,
            double? distanceKm,
            string? coverImageUrl)
        {
            Title = title;
            Description = description;
            DifficultyLevel = difficultyLevel;
            Latitude = latitude;
            Longitude = longitude;
            LocationName = locationName;
            EventDate = eventDate;
            MaxParticipants = maxParticipants;
            DistanceKm = distanceKm;
            CoverImageUrl = coverImageUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Cancel() => Status = EventStatus.Cancelled;
        public void Publish() => Status = EventStatus.Upcoming;
        public void Start() => Status = EventStatus.Ongoing;
        public void Complete() => Status = EventStatus.Completed;
    }
}
