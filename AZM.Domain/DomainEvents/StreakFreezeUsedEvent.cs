using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.DomainEvents
{
    public record StreakFreezeUsedEvent(Guid UserId, int CurrentStreak, int FreezesRemaining) : INotification;
}
