using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Auth.DTOs.Event
{
    public record EventFeedItemDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string SportType { get; init; } = string.Empty;
        public string DifficultyLevel { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public string LocationName { get; init; } = string.Empty;
        public double Latitude { get; init; }
        public double Longitude { get; init; }
        public DateTime EventDate { get; init; }
        public DateTime CreatedAt { get; init; }
        public int ParticipantCount { get; init; }
        public int MaxParticipants { get; init; }
        public bool IsFull { get; init; }
        public double? DistanceKm { get; init; }
        public string? CoverImageUrl { get; init; }
        public OrganizerSummaryDto Organizer { get; init; } = null!;
        public bool IsJoined { get; init; }
    }
}
