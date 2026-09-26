using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.Notification
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        public NotificationCategory Category { get; set; }
        public NotificationActorDto? Actor { get; set; }        // present only when Category == User (or Event, if an actor also exists)
        public Guid? RelatedEventId { get; set; }                // present only when Category == Event
    }
}
