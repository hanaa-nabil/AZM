using AZM.Application.Common;
using MediatR;

namespace AZM.Application.Events.Commands
{
    public class JoinEventCommand : IRequest<Result<bool>>
    {
        public Guid EventId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}