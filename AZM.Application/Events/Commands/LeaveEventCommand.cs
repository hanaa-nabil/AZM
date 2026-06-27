using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Events.Commands
{
    public record LeaveEventCommand(Guid EventId, Guid UserId) : IRequest<Result<bool>>;
}