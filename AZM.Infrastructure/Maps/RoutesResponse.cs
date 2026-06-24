using Microsoft.AspNetCore.Rewrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Maps
{
    public class RoutesResponse
    {
        [JsonPropertyName("routes")]
        public List<RouteResult> Routes { get; set; } = [];
    }

}
