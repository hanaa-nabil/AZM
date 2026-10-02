using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.Squad
{
    public class SquadDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string Privacy { get; set; } = string.Empty;
        public int MemberCount { get; set; }
        public List<SquadMemberDto> Roster { get; set; } = new();
        public List<SquadMemberDto> PendingRequests { get; set; } = new();
    }
}
