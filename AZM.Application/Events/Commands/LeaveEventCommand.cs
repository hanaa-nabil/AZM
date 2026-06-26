using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Events.Commands
{
<<<<<<< HEAD
    public record LeaveEventCommand(Guid EventId, Guid UserId) : IRequest<Result<bool>>;
}
=======
    public class LeaveEventCommand : IRequest<Result<bool>>
    {
        public Guid EventId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
>>>>>>> DB Back to local, Auth working technically
