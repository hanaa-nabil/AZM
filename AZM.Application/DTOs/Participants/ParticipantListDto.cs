using AZM.Application.DTOs.Event;

namespace AZM.Application.DTOs.Participants
{
    public record ParticipantListDto
    {
        public Guid EventId { get; init; }
        public string EventTitle { get; init; } = string.Empty;
        public int TotalJoined { get; init; }
        public IEnumerable<ParticipantDto> Participants { get; init; } = [];
        public OrganizerSummaryDto Organizer { get; set; } = default!;
    }
}
