using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Enums
{
    public enum EventVisibility
    {
        Public = 0,        // anyone can see and join
        FollowersOnly = 1, // only the organizer's followers can see and join
        FemaleOnly = 2     // only female users can see and join
    }
}
