using AZM.Application.Common;
using AZM.Application.Events.Commands;
using AZM.Domain.DomainEvents;
using AZM.Domain.Entities;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class UpdateEventCommandHandler : IRequestHandler<UpdateEventCommand, Result<bool>>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IMediator _mediator;

        public UpdateEventCommandHandler(IEventRepository eventRepo, IMediator mediator)
        {
            _eventRepo = eventRepo;
            _mediator = mediator;
        }

        public async Task<Result<bool>> Handle(UpdateEventCommand request, CancellationToken cancellationToken)
        {
            var ev = await _eventRepo.GetByIdAsync(request.EventId, cancellationToken);
            if (ev is null)
                return Result<bool>.Failure("Event not found");

            if (ev.OrganizerId != request.RequestingUserId)
                return Result<bool>.Failure("Only the organizer can update this event");

            EventRoute? route = request.Route is not null ? new EventRoute
            {
                StartLatitude = request.Route.StartLatitude,
                StartLongitude = request.Route.StartLongitude,
                StartAddress = request.Route.StartAddress,
                EndLatitude = request.Route.EndLatitude,
                EndLongitude = request.Route.EndLongitude,
                EndAddress = request.Route.EndAddress,
                DistanceMeters = request.Route.DistanceMeters,
                EstimatedDurationSeconds = request.Route.EstimatedDurationSeconds,
                Polyline = request.Route.Polyline
            } : null;

            ev.Update(request.Title, request.Description, request.DifficultyLevel,
                request.Latitude, request.Longitude, request.LocationName, request.EventDate,
                request.MaxParticipants, request.DistanceKm, request.CoverImageUrl, request.IsPublic,
                request.Pace, route); 

            await _eventRepo.UpdateAsync(ev, cancellationToken);

            var participants = await _eventRepo.GetParticipantsAsync(ev.Id, cancellationToken);
            var participantIds = participants.Select(p => p.UserId).ToList();

            if (participantIds.Count > 0)
                await _mediator.Publish(new EventUpdated(ev.Id, participantIds), cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
