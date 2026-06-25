using AZM.Application.Auth.DTOs.Event;
using MediatR;

namespace AZM.Application.Events.Queries
{
    public class GetEventByIdQuery : IRequest<EventDto?>
    {
        public Guid EventId { get; set; }
    }
}
