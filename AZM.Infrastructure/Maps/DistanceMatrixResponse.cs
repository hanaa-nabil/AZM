using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Maps
{
    public class DistanceMatrixResponse
    {
        [JsonPropertyName("rows")]
        public List<MatrixRow> Rows { get; set; } = [];
    }

}
