using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Maps
{
    public class MatrixElement
    {
        [JsonPropertyName("duration")]
        public ValueObject Duration { get; set; } = null!;

        // Only present when departure_time is sent
        [JsonPropertyName("duration_in_traffic")]
        public ValueObject? DurationInTraffic { get; set; }

        [JsonPropertyName("distance")]
        public ValueObject Distance { get; set; } = null!;
    }
}
