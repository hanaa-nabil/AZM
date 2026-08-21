using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.DTOs.Event
{
    public class NearbyEventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SportType { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
