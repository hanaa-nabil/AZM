using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Auth.DTOs.Event
{
    public record OrganizerSummaryDto
    {
        public Guid Id { get; init; }
        public string FullName { get; init; } = string.Empty;
        public string? AvatarUrl { get; init; }
    }
}
