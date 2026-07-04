using AZM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid RecipientId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public Guid? RelatedEventId { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }

        public User Recipient { get; set; } = default!;
        public Event? RelatedEvent { get; set; }
    }
}
