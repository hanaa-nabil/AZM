using AZM.Application.Common;
using AZM.Application.Events.Commands;
using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class CreateEventHandler : IRequestHandler<CreateEventCommand, Result<Guid>>
    {
        private readonly IEventRepository _eventRepo;
        public CreateEventHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<Guid>> Handle(CreateEventCommand cmd, CancellationToken ct)
        {
            if (cmd.EventDate <= DateTime.UtcNow)
                return Result<Guid>.Failure("Event date must be in the future.");
            if (cmd.Title.Length < 3)
                return Result<Guid>.Failure("Title must be at least 3 characters.");

            // Build route entity if provided
            EventRoute? route = null;
            if (cmd.Route is not null)
            {
                route = new EventRoute
                {
                    StartLatitude = cmd.Route.StartLatitude,
                    StartLongitude = cmd.Route.StartLongitude,
                    StartAddress = cmd.Route.StartAddress,
                    EndLatitude = cmd.Route.EndLatitude,
                    EndLongitude = cmd.Route.EndLongitude,
                    EndAddress = cmd.Route.EndAddress,
                    DistanceMeters = cmd.Route.DistanceMeters,
                    EstimatedDurationSeconds = cmd.Route.EstimatedDurationSeconds,
                    Polyline = cmd.Route.Polyline
                };
            }

            var ev = Event.Create(
                 title: cmd.Title,
                 description: cmd.Description,
                 sportType: cmd.SportType,
                 difficultyLevel: cmd.DifficultyLevel,
                 latitude: cmd.Latitude,
                 longitude: cmd.Longitude,
                 locationName: cmd.LocationName,
                 eventDate: cmd.EventDate,
                 organizerId: cmd.OrganizerId,
                 maxParticipants: cmd.MaxParticipants,
                 distanceKm: cmd.DistanceKm,
                 routeImageUrl: null,
                 coverImageUrl: cmd.CoverImageUrl,
                 isPublic: cmd.IsPublic,
                 route: route,
                 pace: cmd.Pace);

            await _eventRepo.AddAsync(ev, ct);
            return Result<Guid>.Success(ev.Id);
        }
    }
}