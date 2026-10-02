using AZM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class SquadMember
    {
        public Guid Id { get; private set; }
        public Guid SquadId { get; private set; }
        public Squad Squad { get; private set; } = null!;
        public Guid UserId { get; private set; }
        public User User { get; private set; } = null!;
        public SquadMemberRole Role { get; private set; }
        public SquadMemberStatus Status { get; private set; }
        public DateTime RequestedAt { get; private set; }
        public DateTime? ApprovedAt { get; private set; }

        private SquadMember() { }

        public static SquadMember CreateFounder(Guid squadId, Guid userId)
        {
            return new SquadMember
            {
                Id = Guid.NewGuid(),
                SquadId = squadId,
                UserId = userId,
                Role = SquadMemberRole.Founder,
                Status = SquadMemberStatus.Approved,
                RequestedAt = DateTime.UtcNow,
                ApprovedAt = DateTime.UtcNow
            };
        }

        public static SquadMember RequestToJoin(Guid squadId, Guid userId, bool autoApprove)
        {
            return new SquadMember
            {
                Id = Guid.NewGuid(),
                SquadId = squadId,
                UserId = userId,
                Role = SquadMemberRole.Member,
                Status = autoApprove ? SquadMemberStatus.Approved : SquadMemberStatus.Pending,
                RequestedAt = DateTime.UtcNow,
                ApprovedAt = autoApprove ? DateTime.UtcNow : null
            };
        }

        public void Approve()
        {
            Status = SquadMemberStatus.Approved;
            ApprovedAt = DateTime.UtcNow;
        }

        public void Remove() => Status = SquadMemberStatus.Removed;

        public void PromoteToCoCaptain() => Role = SquadMemberRole.CoCaptain;
        public void DemoteToMember() => Role = SquadMemberRole.Member;
    }
}
