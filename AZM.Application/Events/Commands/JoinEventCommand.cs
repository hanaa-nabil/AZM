using AZM.Application.Common;
using MediatR;


namespace AZM.Application.Events.Commands
{
    public record JoinEventCommand(Guid EventId, Guid UserId) : IRequest<Result<bool>>;
}