using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.User
{
    public class StreakDto
    {
        public int CurrentStreak { get; set; }
        public int LongestStreak { get; set; }
        public int FreezesAvailable { get; set; }
        public int NextMilestone { get; set; }
        public DateOnly? LastActiveDate { get; set; }
        public bool IsAtRiskToday { get; set; } 
    }
}
