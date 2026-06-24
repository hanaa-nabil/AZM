using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Maps
{
    public class RouteResult
    {
        [JsonPropertyName("distanceMeters")]
        public int DistanceMeters { get; set; }

        [JsonPropertyName("duration")]
        public string Duration { get; set; } = string.Empty;  // "1234s"

        [JsonPropertyName("polyline")]
        public EncodedPolyline Polyline { get; set; } = null!;
    }
}
