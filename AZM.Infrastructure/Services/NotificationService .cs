using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using FirebaseAdmin.Messaging;

namespace AZM.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IUserRepository _userRepo; 

        public NotificationService(INotificationRepository repo, IUserRepository userRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
        }

        public async Task SendAsync(Guid recipientId, NotificationType type, string title, string body,
            Guid? relatedEventId = null, CancellationToken ct = default)
        {
            var notification = new Domain.Entities.Notification
            {
                RecipientId = recipientId,
                Type = type,
                Title = title,
                Body = body,
                RelatedEventId = relatedEventId,
                CreatedAt = DateTime.UtcNow,
            };

            await _repo.AddAsync(notification, ct);
            await PushAsync(recipientId, title, body, ct);
        }

        public async Task SendBulkAsync(IEnumerable<Guid> recipientIds, NotificationType type, string title, string body,
            Guid? relatedEventId = null, CancellationToken ct = default)
        {
            var ids = recipientIds.ToList();
            var notifications = ids.Select(id => new Domain.Entities.Notification
            {
                RecipientId = id,
                Type = type,
                Title = title,
                Body = body,
                RelatedEventId = relatedEventId,
                CreatedAt = DateTime.UtcNow,
            });

            await _repo.AddRangeAsync(notifications, ct);

            foreach (var id in ids)
                await PushAsync(id, title, body, ct);
        }

        private async Task PushAsync(Guid userId, string title, string body, CancellationToken ct)
        {
            var user = await _userRepo.GetByIdAsync(userId.ToString());
            if (user?.FcmToken is null) return;

            var message = new Message
            {
                Token = user.FcmToken,
                Notification = new FirebaseAdmin.Messaging.Notification { Title = title, Body = body }
            };

            try
            {
                await FirebaseMessaging.DefaultInstance.SendAsync(message, ct);
            }
            catch
            {
                // token invalid/expired — log and optionally clear it from the user record
            }
        }
        public async Task SendToGroupAsync(IEnumerable<string> userIds, string title, string body, CancellationToken ct = default)
        {
            foreach (var idStr in userIds)
            {
                if (!Guid.TryParse(idStr, out var userId)) continue;
                await SendAsync(userId, NotificationType.EventStartingSoon, title, body, ct: ct);
            }
        }
    }
}