using AZM.Application.DTOs.Notification;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Notifications.Queries
{
    public class GetMyNotificationsHandler : IRequestHandler<GetMyNotificationsQuery, List<NotificationDto>>
    {
        private readonly INotificationRepository _repo;

        public GetMyNotificationsHandler(INotificationRepository repo) => _repo = repo;

        public async Task<List<NotificationDto>> Handle(GetMyNotificationsQuery request, CancellationToken ct)
        {
            var notifications = await _repo.GetForUserAsync(request.UserId, request.Page, request.PageSize, ct);

            return notifications.Select(n => new NotificationDto(
                n.Id, n.Type.ToString(), n.Title, n.Body, n.RelatedEventId, n.IsRead, n.CreatedAt
            )).ToList();
        }
    }
}
