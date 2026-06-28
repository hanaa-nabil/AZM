using AZM.Application.Auth.DTOs.Event;
using AZM.Application.Common;
using AZM.Domain.Enums;
using MediatR;

namespace AZM.Application.Events.Commands
{
    public record CreateEventCommand(
        string Title,
        string Description,
        SportType SportType,
        DifficultyLevel DifficultyLevel,
        double Latitude,
        double Longitude,
        string LocationName,
        DateTime EventDate,
        Guid OrganizerId,
        int MaxParticipants = 0,
        double? DistanceKm = null,
        string? CoverImageUrl = null,
        bool IsPublic = true,
        EventRouteDto? Route = null
    ) : IRequest<Result<Guid>>;
}