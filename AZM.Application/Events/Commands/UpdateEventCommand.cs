using AZM.Application.Auth.DTOs.Event;
using AZM.Application.Common;
using AZM.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Commands
{
    public record UpdateEventCommand(
        Guid EventId,
        Guid RequestingUserId,
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
        EventRouteDto? Route = null
    ) : IRequest<Result<bool>>;
}
