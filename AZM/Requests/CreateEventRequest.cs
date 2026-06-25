using AZM.Domain.Enums;

namespace AZM.Api.Requests
{
    public class CreateEventRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartAtUtc { get; set; }
        public DifficultyLevel Difficulty { get; set; }
        public int? MaxParticipants { get; set; }
        public double MeetingLat { get; set; }
        public double MeetingLng { get; set; }
        public string MeetingAddress { get; set; } = string.Empty;
        public SportType SportType { get; set; }
    }
}