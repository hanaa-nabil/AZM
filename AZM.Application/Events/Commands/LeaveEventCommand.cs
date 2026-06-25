using AZM.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Queries
{
    public class LeaveEventCommand : IRequest<Result<bool>>
    {
        public Guid EventId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
