using AZM.Application.Notifications.Commands;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Notifications.Handlers
{
    public class MarkAllNotificationsReadHandler : IRequestHandler<MarkAllNotificationsReadCommand>
    {
        private readonly INotificationRepository _notificationRepository;

        public MarkAllNotificationsReadHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationRepository.MarkAllReadAsync(request.UserId);
        }
    }
}
