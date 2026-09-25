using AZM.Application.Common;
using AZM.Application.Users.Commands;
using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Users.Handlers
{
    public class CompleteEventActivityHandler : IRequestHandler<CompleteEventActivityCommand, Result<bool>>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IUserRepository _userRepository;

        public CompleteEventActivityHandler(IEventRepository eventRepo, IUserRepository userRepository)
        {
            _eventRepo = eventRepo;
            _userRepository = userRepository;
        }

        public async Task<Result<bool>> Handle(CompleteEventActivityCommand cmd, CancellationToken ct)
        {
            if (cmd.Steps < 0 || cmd.DistanceMeters < 0)
                return Result<bool>.Failure("Steps and distance must be non-negative.");

            var ev = await _eventRepo.GetByIdAsync(cmd.EventId, ct);
            if (ev is null) return Result<bool>.Failure("Event not found.");

            var participant = await _eventRepo.GetParticipantAsync(cmd.EventId, cmd.UserId, ct);
            if (participant is null || participant.Status != ParticipantStatus.Joined)
                return Result<bool>.Failure("You are not a participant of this event.");

            if (participant.HasCompletedActivity)
                return Result<bool>.Failure("You've already completed this event.");

            if (ev.EventDate.Date != DateTime.UtcNow.Date)
                return Result<bool>.Failure("This event is not scheduled for today.");

            if (ev.EventDate > DateTime.UtcNow)
                return Result<bool>.Failure("The event hasn't started yet.");

            var user = await _userRepository.GetByIdWithDetailsAsync(cmd.UserId);
            if (user?.Profile is null)
                return Result<bool>.Failure("User profile not found.", 404);

            user.Profile.TotalStepCount += cmd.Steps;
            user.Profile.TotalDistanceMeters += cmd.DistanceMeters;
            user.Profile.EventsCompletedCount++;

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            user.Profile.RegisterActivity(today);
            await _userRepository.RecordDailyActivityAsync(cmd.UserId, today);
            await _userRepository.UpdateAsync(user);

            participant.MarkActivityCompleted();
            await _eventRepo.UpdateParticipantAsync(participant, ct);

            return Result<bool>.Success(true);
        }
    }
}
