using AZM.Application.Common;
using AZM.Application.Events.Commands;
using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class CreateEventHandler : IRequestHandler<CreateEventCommand, Result<Guid>>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IUserRepository _userRepo;
        public CreateEventHandler(IEventRepository eventRepo, IUserRepository userRepo)
        {
            _eventRepo = eventRepo;
            _userRepo = userRepo;
        }

        public async Task<Result<Guid>> Handle(CreateEventCommand cmd, CancellationToken ct)
        {
            if (cmd.EventDate <= DateTime.UtcNow)
                return Result<Guid>.Failure("Event date must be in the future.");
            if (cmd.Title.Length < 3)
                return Result<Guid>.Failure("Title must be at least 3 characters.");

            var user = await _userRepo.GetByIdAsync(cmd.OrganizerId.ToString());

            if (user is null)
                return Result<Guid>.Failure("User not found.");

            if (cmd.IsPink && user.Gender != Gender.Female)
                return Result<Guid>.Failure("Only women can create pink events.");
            if (cmd.IsPink && user.Gender != Gender.Female)
                return Result<Guid>.Failure("Only women can create pink events.");

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
                 routeImageUrl: null,
                 coverImageUrl: cmd.CoverImageUrl,
                 isPrivate: cmd.IsPrivate,
                 isPink: cmd.IsPink,
                 route: route,
                 pace: cmd.Pace);

            await _eventRepo.AddAsync(ev, ct);
            return Result<Guid>.Success(ev.Id);
        }
    }
}