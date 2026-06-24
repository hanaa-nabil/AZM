using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Maps
{
    public class ValueObject
    {
        [JsonPropertyName("value")]
        public int Value { get; set; }   // seconds or meters

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;  // "12 mins", "3.2 km"
    }
}
