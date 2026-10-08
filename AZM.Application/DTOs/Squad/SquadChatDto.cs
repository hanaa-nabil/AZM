using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.Squad
{
    public class SquadChatDto
    {
        public string SquadName { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public int OnlineCount { get; set; }
        public PinnedEventDto? PinnedEvent { get; set; }
        public List<SquadMessageDto> Messages { get; set; } = new();
    }
}
