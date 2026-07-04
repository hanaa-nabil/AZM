using MediatR;

namespace AZM.Domain.DomainEvents
{
    public record EventPublished(Guid EventId, Guid OrganizerId) : INotification;
}
