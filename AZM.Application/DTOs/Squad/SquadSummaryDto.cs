using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.Squad
{
    public class SquadSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public int MemberCount { get; set; }
        public string Privacy { get; set; } = string.Empty;
        public List<string> Sports { get; set; } = new();
        public string? LocationName { get; set; }
    }

}
