using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Events.Commands
{
    public record PublishEventCommand(Guid EventId, Guid RequestingUserId) : IRequest<Result<bool>>;
}
