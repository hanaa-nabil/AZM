using AZM.Domain.Enums;
using System.Text.Json.Serialization;

namespace AZM.Application.Auth.DTOs.Event
{
    public class EventSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; }
        public EventStatus Status { get; set; }
        public DateTime StartAtUtc { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SportType SportType { get; set; }

        public string CreatedByUserId { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
        public double MeetingLat { get; set; }
        public double MeetingLng { get; set; }
        public string MeetingAddress { get; set; } = string.Empty;
        public int EstimatedMinutes { get; set; }
        public int? MaxParticipants { get; set; }
        public int ParticipantCount { get; set; }

        public string ParticipantsSummary =>
            MaxParticipants.HasValue
                ? $"{ParticipantCount}/{MaxParticipants}"
                : $"{ParticipantCount} joined";

        public bool IsFull =>
            MaxParticipants.HasValue &&
            ParticipantCount >= MaxParticipants.Value;
    }
}