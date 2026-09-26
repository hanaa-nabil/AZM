using AZM.Domain.Enums;
using AZM.Domain.Interfaces;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace AZM.Infrastructure.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository repo,
            IUserRepository userRepo,
            ILogger<NotificationService> logger)
        {
            _repo = repo;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task SendAsync(Guid recipientId, NotificationType type, string title, string body,
            Guid? relatedEventId = null, Guid? actorId = null, CancellationToken ct = default)
        {
            var notification = new Domain.Entities.Notification
            {
                RecipientId = recipientId,
                ActorId = actorId,
                Type = type,
                Title = title,
                Body = body,
                RelatedEventId = relatedEventId,
                CreatedAt = DateTime.UtcNow,
            };

            await _repo.AddAsync(notification, ct);
            // notification.Id is now populated by EF after SaveChanges, since the DB generates it
            // and EF Core reads the generated value back into the tracked entity automatically.
            await PushAsync(recipientId, title, body, notification.Id, type.ToString(), relatedEventId, actorId, ct);
        }

        public async Task SendBulkAsync(IEnumerable<Guid> recipientIds, NotificationType type, string title, string body,
            Guid? relatedEventId = null, Guid? actorId = null, CancellationToken ct = default)
        {
            var notifications = recipientIds.Select(id => new Domain.Entities.Notification
            {
                RecipientId = id,
                ActorId = actorId,
                Type = type,
                Title = title,
                Body = body,
                RelatedEventId = relatedEventId,
                CreatedAt = DateTime.UtcNow,
            }).ToList();

            await _repo.AddRangeAsync(notifications, ct);
            // Same as above — each notification's Id is populated post-save by EF.

            foreach (var n in notifications)
                await PushAsync(n.RecipientId, title, body, n.Id, type.ToString(), relatedEventId, actorId, ct);
        }

        private async Task PushAsync(
            Guid userId, string title, string body,
            Guid notificationId, string type, Guid? relatedEventId, Guid? actorId,
            CancellationToken ct)
        {
            var user = await _userRepo.GetByIdAsync(userId.ToString());
            if (string.IsNullOrEmpty(user?.FcmToken))
            {
                _logger.LogDebug("Skipping push for user {UserId} — no FCM token registered.", userId);
                return;
            }

            var data = new Dictionary<string, string>
            {
                ["notificationId"] = notificationId.ToString(),
                ["type"] = type,
            };
            if (relatedEventId.HasValue) data["relatedEventId"] = relatedEventId.Value.ToString();
            if (actorId.HasValue) data["actorId"] = actorId.Value.ToString();

            var message = new Message
            {
                Token = user.FcmToken,
                Notification = new FirebaseAdmin.Messaging.Notification { Title = title, Body = body },
                Data = data,
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification { ChannelId = "default_channel" }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps { ContentAvailable = true }
                }
            };

            try
            {
                await FirebaseMessaging.DefaultInstance.SendAsync(message, ct);
                _logger.LogInformation(
                    "Push sent to user {UserId} for notification {NotificationId} ({Type}).",
                    userId, notificationId, type);
            }
            catch (FirebaseMessagingException ex) when (
                ex.MessagingErrorCode == MessagingErrorCode.Unregistered ||
                ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
            {
                // Token is dead (uninstalled app, expired, or malformed) — clear it so we
                // stop wasting calls on it on every future notification for this user.
                _logger.LogWarning(
                    "FCM token for user {UserId} is invalid/unregistered ({ErrorCode}). Clearing stored token.",
                    userId, ex.MessagingErrorCode);
                await _userRepo.ClearFcmTokenAsync(userId, user.FcmToken);
            }
            catch (FirebaseMessagingException ex)
            {
                // Transient failure (rate limiting, Firebase server error, network issue, etc.)
                // — don't clear the token, it may still be valid; just log and move on so
                // this doesn't block or fail the rest of the notification flow.
                _logger.LogError(ex,
                    "Transient FCM error sending push to user {UserId} ({ErrorCode}). Token kept.",
                    userId, ex.MessagingErrorCode);
            }
            catch (Exception ex)
            {
                // Unexpected error unrelated to FCM itself (e.g. serialization issue) —
                // log with full detail so it's traceable, but never let a push failure
                // bubble up and fail the notification-creation flow that already succeeded.
                _logger.LogError(ex, "Unexpected error sending push notification to user {UserId}.", userId);
            }
        }

        public async Task SendToGroupAsync(IEnumerable<string> userIds, string title, string body, CancellationToken ct = default)
        {
            foreach (var idStr in userIds)
            {
                if (!Guid.TryParse(idStr, out var userId))
                {
                    _logger.LogWarning("SendToGroupAsync received an invalid user id string: {IdStr}", idStr);
                    continue;
                }
                await SendAsync(userId, NotificationType.EventStartingSoon, title, body, ct: ct);
            }
        }
    }
}