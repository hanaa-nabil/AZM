using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class Follow
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid FollowerId { get; set; }   // the person doing the following
        public Guid FollowingId { get; set; }  // the person being followed
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
