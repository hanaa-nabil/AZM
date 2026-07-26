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
    public class DeleteAllNotificationsHandler : IRequestHandler<DeleteAllNotificationsCommand>
    {
        private readonly INotificationRepository _notificationRepository;

        public DeleteAllNotificationsHandler(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task Handle(DeleteAllNotificationsCommand request, CancellationToken cancellationToken)
        {
            await _notificationRepository.DeleteAllAsync(request.UserId);
        }
    }
}
