using AZM.Domain.DomainEvents;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Notifications.Handler
{
    public class NotifyOnEventCancelledHandler : INotificationHandler<EventCancelled>
    {
        private readonly INotificationService _notificationService;

        public NotifyOnEventCancelledHandler(INotificationService notificationService)
            => _notificationService = notificationService;

        public Task Handle(EventCancelled e, CancellationToken ct)
            => _notificationService.SendBulkAsync(
                e.ParticipantIds,
                NotificationType.EventCancelled,
                "Event cancelled",
                "An event you joined was cancelled",
                e.EventId,
                ct);
    }
}
