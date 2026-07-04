using AZM.Application.DTOs.Participants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.Event
{
    public record EventDetailDto : EventFeedItemDto
    {
        public string? RouteImageUrl { get; init; }
        public IEnumerable<ParticipantDto> Participants { get; init; } = [];
    }
}
