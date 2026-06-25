using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.BackgroundJobs
{
    public class EventReminderJob      
    {
        private readonly IEventRepository _eventRepo;
        private readonly INotificationService _notifications;

        public EventReminderJob(
            IEventRepository eventRepo,
            INotificationService notifications)
        {
            _eventRepo = eventRepo;
            _notifications = notifications;
        }

        public async Task RunAsync()
        {
            var events = await _eventRepo.GetStartingWithinAsync(TimeSpan.FromHours(1));

            foreach (var ev in events)
            {
                var participantIds = ev.Participants
                    .Where(p => p.Status == ParticipantStatus.Joined)
                    .Select(p => p.UserId);

                await _notifications.SendToGroupAsync(
                    participantIds,
                    "Event starting soon!",
                    $"{ev.Title} starts in 1 hour. Get ready!");
            }
        }
    }

}
