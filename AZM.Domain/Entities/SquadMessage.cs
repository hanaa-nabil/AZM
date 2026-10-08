using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class SquadMessage
    {
        public Guid Id { get; private set; }
        public Guid SquadId { get; private set; }
        public Squad Squad { get; private set; } = null!;
        public Guid SenderId { get; private set; }
        public User Sender { get; private set; } = null!;
        public string Content { get; private set; } = string.Empty;
        public DateTime SentAt { get; private set; }
        public bool IsDeleted { get; private set; }

        private SquadMessage() { }

        public static SquadMessage Create(Guid squadId, Guid senderId, string content) => new()
        {
            Id = Guid.NewGuid(),
            SquadId = squadId,
            SenderId = senderId,
            Content = content,
            SentAt = DateTime.UtcNow
        };

        public void SoftDelete() { IsDeleted = true; Content = string.Empty; }
    }
}
