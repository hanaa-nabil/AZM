using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Enums
{
    public enum NotificationType
    {
        EventPublished,
        EventUpdated,
        EventCancelled,
        EventStartingSoon,
        ParticipantJoined,
        ParticipantLeft,
        RemovedFromEvent,
        WaitlistPromoted,
        StreakDanger ,
        StreakMilestone,
        StreakBroken ,
        StreakFreezeUsed ,
        BadgeEarned 
    }
}
