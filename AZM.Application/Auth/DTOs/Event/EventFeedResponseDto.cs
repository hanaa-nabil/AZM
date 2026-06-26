using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Auth.DTOs.Event
{
    public record EventFeedResponseDto
    {
        public IEnumerable<EventFeedItemDto> Events { get; init; } = [];
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
        public bool HasMore { get; init; }
    }
}
