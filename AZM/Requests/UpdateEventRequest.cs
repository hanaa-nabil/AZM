using AZM.Domain.Enums;

namespace AZM.Api.Requests
{
    public record UpdateEventRequest(
       string Title,
       string Description,
       DifficultyLevel DifficultyLevel,
       double Latitude,
       double Longitude,
       string LocationName,
       DateTime EventDate,
       int MaxParticipants,
       double? DistanceKm,
       string? CoverImageUrl,
       bool IsPublic = true,
       EventRouteRequest? Route = null
   );
}
