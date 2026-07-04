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
    public class MarkNotificationReadHandler : IRequestHandler<MarkNotificationReadCommand>
    {
        private readonly INotificationRepository _repo;

        public MarkNotificationReadHandler(INotificationRepository repo) => _repo = repo;

        public Task Handle(MarkNotificationReadCommand request, CancellationToken ct)
            => _repo.MarkAsReadAsync(request.NotificationId, request.UserId, ct);
    }
}
