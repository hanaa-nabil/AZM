using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Entities
{
    public class EventRoute
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public double StartLatitude { get; set; }
        public double StartLongitude { get; set; }
        public string? StartAddress { get; set; }

        public double EndLatitude { get; set; }
        public double EndLongitude { get; set; }
        public string? EndAddress { get; set; }

        public double? DistanceMeters { get; set; }
        public int? EstimatedDurationSeconds { get; set; }

        public ICollection<EventRouteWaypoint> Waypoints { get; set; } = new List<EventRouteWaypoint>();

        public Guid EventId { get; set; }
        public Event? Event { get; set; }
    }

      public class EventRouteWaypoint
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Order { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public Guid EventRouteId { get; set; }
        public EventRoute? EventRoute { get; set; }
    }
}
