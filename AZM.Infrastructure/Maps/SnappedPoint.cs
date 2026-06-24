using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Maps
{
    public class SnappedPoint
    {
        [JsonPropertyName("location")]
        public LatLngLiteral Location { get; set; } = null!;
    }
}
