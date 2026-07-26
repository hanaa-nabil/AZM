using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Notifications.Commands
{
    public record MarkAllNotificationsReadCommand(Guid UserId) : IRequest;
}
