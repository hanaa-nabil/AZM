using AZM.Application.Common;
using AZM.Application.Events.Commands;
using AZM.Domain.Interfaces;
using MediatR;

namespace AZM.Application.Events.Handlers
{
    public class UpdateEventHandler : IRequestHandler<UpdateEventCommand, Result<bool>>
    {
        private readonly IEventRepository _eventRepo;

        public UpdateEventHandler(IEventRepository eventRepo) => _eventRepo = eventRepo;

        public async Task<Result<bool>> Handle(UpdateEventCommand cmd, CancellationToken ct)
        {
            var ev = await _eventRepo.GetByIdAsync(cmd.EventId, ct);
            if (ev is null) return Result<bool>.Failure("Event not found.");
            if (ev.OrganizerId != cmd.RequestingUserId)
                return Result<bool>.Failure("Only the organizer can update this event.");

            ev.Update(cmd.Title, cmd.Description, cmd.DifficultyLevel,
                cmd.Latitude, cmd.Longitude, cmd.LocationName,
                cmd.EventDate, cmd.MaxParticipants, cmd.DistanceKm, cmd.CoverImageUrl);

            await _eventRepo.UpdateAsync(ev, ct);
            return Result<bool>.Success(true);
        }
    }
}
