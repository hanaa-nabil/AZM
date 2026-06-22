using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class LiveLocation
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Optional extras commonly sent by GPS pings
        public double? AccuracyMeters { get; set; }
        public double? SpeedMetersPerSecond { get; set; }
        public double? HeadingDegrees { get; set; }

        public DateTime RecordedAtUtc { get; set; } = DateTime.UtcNow;

        // ----- Back-reference to the tracking session this ping belongs to -----
        public Guid LiveSessionId { get; set; }
        public LiveSession? LiveSession { get; set; }
    }
}
