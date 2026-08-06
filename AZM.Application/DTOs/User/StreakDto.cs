using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.User
{
  
    public class StreakDto
    {
        [JsonPropertyName("streak_count")]
        public int StreakCount { get; set; }

        [JsonPropertyName("freeze_count")]
        public int FreezeCount { get; set; }

        [JsonPropertyName("days")]
        public List<StreakDayDto> Days { get; set; } = new();
    }
}
