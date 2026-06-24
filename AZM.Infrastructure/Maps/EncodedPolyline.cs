    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Maps
{
    public class EncodedPolyline
    {
        [JsonPropertyName("encodedPolyline")]
        public string Value { get; set; } = string.Empty;
    }
}
