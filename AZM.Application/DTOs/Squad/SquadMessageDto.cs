using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.Squad
{
    public class SquadMessageDto
    {
        public Guid Id { get; set; }
        public Guid SquadId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string? SenderAvatar { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsDeleted { get; set; }
        public bool Seen { get; set; }   // someone other than the sender has read it (the double tick)
    }
}
