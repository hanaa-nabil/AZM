using MediatR;

namespace AZM.Domain.DomainEvents
{
    public record EventParticipantLeft
        (Guid EventId, Guid OrganizerId, Guid ParticipantId) : INotification;
}
