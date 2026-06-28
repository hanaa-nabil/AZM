using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Auth.DTOs.Event
{
    public record EventRouteDto(
        double StartLatitude,
        double StartLongitude,
        string? StartAddress,
        double EndLatitude,
        double EndLongitude,
        string? EndAddress,
        double? DistanceMeters,
        int? EstimatedDurationSeconds,
        List<WaypointDto>? Waypoints
    );
}
