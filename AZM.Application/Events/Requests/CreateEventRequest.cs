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
        Pace Pace ,
        bool IsPrivate,
         bool IsPink,
        int MaxParticipants = 0,
        string? CoverImageUrl = null,
        EventRouteRequest? Route = null
    );
}
