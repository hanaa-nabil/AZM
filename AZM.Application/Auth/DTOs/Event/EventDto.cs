using AZM.Application.Auth.DTOs.Participants;
using AZM.Domain.Enums;
using System.Text.Json.Serialization;

namespace AZM.Application.Auth.DTOs.Event
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; }
        public EventStatus Status { get; set; }
        public DateTime StartAtUtc { get; set; }
        public double MeetingLat { get; set; }
        public double MeetingLng { get; set; }
        public string MeetingAddress { get; set; } = string.Empty;
        public int? MaxParticipants { get; set; }
        public int ParticipantCount { get; set; }
        public List<ParticipantDto> Participants { get; set; } = new();

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SportType SportType { get; set; }

        public bool IsFull { get; set; }
        public string CreatedByUserId { get; set; } = string.Empty;
        public string CreatedByName { get; set; } = string.Empty;
    }
}