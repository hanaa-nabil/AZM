using AZM.Domain.Enums;


namespace AZM.Application.Auth.DTOs.Participants
{
    public class ParticipantDto
    {
        public Guid UserId { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string? AvatarUrl { get; init; }
        public DateTime JoinedAt { get; init; }
        public string Status { get; init; } = string.Empty;
    }
}