using AZM.Domain.Entities;
using MediatR;

namespace AZM.Domain.DomainEvents
{
    public record EventParticipantJoined(Guid EventId, Guid OrganizerId, Guid ParticipantId) : INotification;
}
