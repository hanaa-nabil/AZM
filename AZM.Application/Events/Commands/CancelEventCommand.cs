using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Events.Commands
{
    public record CancelEventCommand(Guid EventId, Guid RequestingUserId): IRequest<Result<bool>>;
}