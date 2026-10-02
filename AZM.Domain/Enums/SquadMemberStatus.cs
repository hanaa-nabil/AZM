using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Enums
{
    public enum SquadMemberStatus
    {
        Pending = 0,   // requested to join a private squad, awaiting approval
        Approved = 1,
        Removed = 2
    }
}
