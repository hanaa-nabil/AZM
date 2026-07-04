using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.DomainEvents
{
    public record EventCancelled(Guid EventId, List<Guid> ParticipantIds) : INotification;
}
