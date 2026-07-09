using AZM.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.BackgroundJobs
{
    public class EventStatusUpdateJob
    {
        private readonly IEventRepository _eventRepo;
        private readonly IConfiguration _config;
        private readonly ILogger<EventStatusUpdateJob> _logger;

        public EventStatusUpdateJob(
            IEventRepository eventRepo,
            IConfiguration config,
            ILogger<EventStatusUpdateJob> logger)
        {
            _eventRepo = eventRepo;
            _config = config;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var durationHours = _config.GetValue<double>("EventSettings:DefaultEventDurationHours", 3);
            var now = DateTime.UtcNow;

            // Upcoming -> Ongoing
            var toStart = await _eventRepo.GetEventsToStartAsync(now, CancellationToken.None);
            foreach (var ev in toStart)
            {
                ev.Start();
                await _eventRepo.UpdateAsync(ev, CancellationToken.None);
            }
            if (toStart.Count > 0)
                _logger.LogInformation("Started {Count} event(s).", toStart.Count);

            // Ongoing -> Completed
            var completeCutoff = now.AddHours(-durationHours);
            var toComplete = await _eventRepo.GetEventsToCompleteAsync(completeCutoff, CancellationToken.None);
            foreach (var ev in toComplete)
            {
                ev.Complete();
                await _eventRepo.UpdateAsync(ev, CancellationToken.None);
            }
            if (toComplete.Count > 0)
                _logger.LogInformation("Completed {Count} event(s).", toComplete.Count);
        }
    }
}
