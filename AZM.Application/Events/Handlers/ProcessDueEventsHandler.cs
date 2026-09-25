using AZM.Application.Events.Commands;
using AZM.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Application.Events.Handlers
{
    public class ProcessDueEventsHandler : IRequestHandler<ProcessDueEventsCommand, int>
    {
        private readonly IEventRepository _eventRepo;
        private readonly IUserRepository _userRepository;

        public ProcessDueEventsHandler(IEventRepository eventRepo, IUserRepository userRepository)
        {
            _eventRepo = eventRepo;
            _userRepository = userRepository;
        }

        public async Task<int> Handle(ProcessDueEventsCommand request, CancellationToken ct)
        {
            var now = DateTime.UtcNow;
            int processed = 0;

            // Upcoming → Ongoing (event's start time has arrived)
            var toStart = await _eventRepo.GetEventsToStartAsync(now, ct);
            foreach (var ev in toStart)
            {
                ev.Start();
                await _eventRepo.UpdateAsync(ev, ct);
                processed++;
            }

            // Ongoing → Completed (give a grace window after EventDate, e.g. 2 hours, before auto-completing)
            var cutoff = now.AddHours(-2);
            var toComplete = await _eventRepo.GetEventsToCompleteAsync(cutoff, ct);

            foreach (var ev in toComplete)
            {
                var participants = await _eventRepo.GetParticipantsAsync(ev.Id, ct);

                foreach (var p in participants)
                {
                    // Anyone who already checked in via location was already credited —
                    // don't double count them here.
                    if (p.HasCompletedActivity)
                        continue;

                    var user = await _userRepository.GetByIdWithDetailsAsync(p.UserId);
                    if (user?.Profile is null) continue;

                    // No streak/RegisterActivity here — this is the un-verified fallback path,
                    // only checked-in attendance (CompleteEventActivityHandler) earns the streak.
                    user.Profile.EventsCompletedCount++;
                    await _userRepository.UpdateAsync(user);
                }

                ev.Complete();
                await _eventRepo.UpdateAsync(ev, ct);
                processed++;
            }

            return processed;
        }
    }
}
