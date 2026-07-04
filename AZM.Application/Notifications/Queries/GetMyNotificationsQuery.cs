using AZM.Application.DTOs.Notification;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Notifications.Queries
{
    public record GetMyNotificationsQuery(Guid UserId, int Page = 1, int PageSize = 20)
     : IRequest<List<NotificationDto>>;
}
