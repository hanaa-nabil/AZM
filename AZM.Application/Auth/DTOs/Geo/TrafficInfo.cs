using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Auth.DTOs.Geo
{
    public class TrafficInfo
    {
        public int NormalMinutes { get; init; }
        public int TrafficMinutes { get; init; }
        public bool IsCongested { get; init; }
        public int DelayMinutes { get; init; }

        // Convenience label for the mobile UI
        public string CongestionLevel => DelayMinutes switch
        {
            0 => "Clear",
            <= 5 => "Light traffic",
            <= 15 => "Moderate traffic",
            _ => "Heavy traffic"
        };
    }
}
