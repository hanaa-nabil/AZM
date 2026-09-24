using AZM.Domain.Enums;


namespace AZM.Application.DTOs.Participants
{
    public class ParticipantDto
    {
        public Guid Id { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string? AvatarUrl { get; init; }
        public DateTime JoinedAt { get; init; }
        public string Username { get; init; } = string.Empty;

        public string Status { get; init; } = string.Empty;
        public bool IsVerified { get; set; }
    }
}