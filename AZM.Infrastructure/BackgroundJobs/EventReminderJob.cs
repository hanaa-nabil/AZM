using AZM.Domain.Enums;
using AZM.Domain.Interfaces;

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
            var events = await _eventRepo.GetEventsWithPendingRemindersAsync(TimeSpan.FromHours(1));

            foreach (var ev in events)
            {
                var pending = ev.Participants.ToList();
                var minutesLeft = Math.Max(1, (int)Math.Ceiling((ev.EventDate - DateTime.UtcNow).TotalMinutes));

                if (pending.Count > 0)
                {
                    await _notifications.SendBulkAsync(
                        pending.Select(p => p.UserId),
                        NotificationType.EventStartingSoon,
                        "Event starting soon!",
                        $"{ev.Title} starts in {minutesLeft} minutes. Get ready!",
                        relatedEventId: ev.Id,
                        actorId: ev.OrganizerId
                        //squadId: ev.squadid,
                        );

                    foreach (var p in pending)
                    {
                        p.MarkReminderSent();
                        await _eventRepo.UpdateParticipantAsync(p);
                    }
                }

                if (ev.OrganizerReminderSentAt is null)
                {
                    await _notifications.SendAsync(
                        ev.OrganizerId,
                        NotificationType.EventStartingSoon,
                        "Your event is starting soon!",
                        $"{ev.Title} starts in {minutesLeft} minutes.",
                        relatedEventId: ev.Id);

                    ev.MarkOrganizerReminderSent();
                    await _eventRepo.UpdateAsync(ev);
                }
            }
        }
    }
}