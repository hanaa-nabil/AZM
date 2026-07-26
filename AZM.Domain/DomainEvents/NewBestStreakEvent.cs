using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.DomainEvents
{
    public record NewBestStreakEvent(Guid UserId, int NewLongestStreak) : INotification;
}
