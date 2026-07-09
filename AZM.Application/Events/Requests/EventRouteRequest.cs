namespace AZM.Api.Requests
{
    public record EventRouteRequest(
      double StartLatitude,
      double StartLongitude,
      string? StartAddress,
      double EndLatitude,
      double EndLongitude,
      string? EndAddress,
      double? DistanceMeters,
      int? EstimatedDurationSeconds,
      List<WaypointRequest>? Waypoints = null
  );
}
