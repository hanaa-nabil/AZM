using AZM.Domain.Enums;

namespace AZM.Api.Requests
{
    public record CreateEventRequest(
           string Title,
           string Description,
           SportType SportType,
           DifficultyLevel DifficultyLevel,
           double Latitude,
           double Longitude,
           string LocationName,
           DateTime EventDate,
           int MaxParticipants = 0,
           double? DistanceKm = null,
           string? CoverImageUrl = null
       );
}