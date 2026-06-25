using AZM.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Commands
{
    public class PublishEventCommand : IRequest<Result<bool>>
    {
        public Guid EventId { get; set; }
        public string RequestingUserId { get; set; } = string.Empty;
    }
}
