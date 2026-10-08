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
            Guid? relatedEventId = null, Guid? actorId = null, Guid? squadId = null, CancellationToken ct = default)
        {
            var notification = new Domain.Entities.Notification
            {
                RecipientId = recipientId,
                ActorId = actorId,
                Type = type,
                Title = title,
                Body = body,
                RelatedEventId = relatedEventId,
                RelatedSquadId = squadId,
                CreatedAt = DateTime.UtcNow,
            };

            await _repo.AddAsync(notification, ct);

            // NOTE: notification.Id is only populated here if AddAsync calls SaveChangesAsync
            // (or the Id is generated client-side). Verify this in the repository.
            await PushAsync(recipientId, title, body, notification.Id, type.ToString(),
                relatedEventId, actorId, squadId, ct);
        }

        public async Task SendBulkAsync(IEnumerable<Guid> recipientIds, NotificationType type, string title, string body,
            Guid? relatedEventId = null, Guid? actorId = null, Guid? squadId = null, CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;

            var notifications = recipientIds
                .Distinct()
                .Select(id => new Domain.Entities.Notification
                {
                    RecipientId = id,
                    ActorId = actorId,
                    Type = type,
                    Title = title,
                    Body = body,
                    RelatedEventId = relatedEventId,
                    RelatedSquadId = squadId,
                    CreatedAt = now,
                })
                .ToList();

            if (notifications.Count == 0)
                return;

            await _repo.AddRangeAsync(notifications, ct);

            foreach (var n in notifications)
            {
                ct.ThrowIfCancellationRequested();
                await PushAsync(n.RecipientId, title, body, n.Id, type.ToString(),
                    relatedEventId, actorId, squadId, ct);
            }
        }

        public async Task SendToGroupAsync(IEnumerable<string> userIds, string title, string body, CancellationToken ct = default)
        {
            var ids = new List<Guid>();
            foreach (var idStr in userIds)
            {
                if (Guid.TryParse(idStr, out var userId))
                    ids.Add(userId);
                else
                    _logger.LogWarning("SendToGroupAsync received an invalid user id string: {IdStr}", idStr);
            }

            await SendBulkAsync(ids, NotificationType.EventStartingSoon, title, body, ct: ct);
        }

        private async Task PushAsync(Guid userId, string title, string body, Guid notificationId,
            string type, Guid? relatedEventId, Guid? actorId, Guid? squadId, CancellationToken ct)
        {
            try
            {
                var user = await _userRepo.GetByIdAsync(userId.ToString());
                if (string.IsNullOrEmpty(user?.FcmToken))
                {
                    _logger.LogDebug("Skipping push for user {UserId} — no FCM token registered.", userId);
                    return;
                }

                var category = squadId.HasValue ? "Squad"
                             : relatedEventId.HasValue ? "Event"
                             : actorId.HasValue ? "User"
                             : "General";

                var data = new Dictionary<string, string>
                {
                    ["notificationId"] = notificationId.ToString(),
                    ["type"] = type,
                    ["category"] = category,
                };
                if (relatedEventId.HasValue) data["relatedEventId"] = relatedEventId.Value.ToString();
                if (actorId.HasValue) data["actorId"] = actorId.Value.ToString();
                if (squadId.HasValue) data["squadId"] = squadId.Value.ToString();

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
                        Aps = new Aps { ContentAvailable = true, Sound = "default" }
                    }
                };

                try
                {
                    await FirebaseMessaging.DefaultInstance.SendAsync(message, ct);

                    _logger.LogInformation(
                        "Push sent to user {UserId} for notification {NotificationId} ({Type}).",
                        userId, notificationId, type);
                }
                catch (FirebaseMessagingException ex) when (ex.MessagingErrorCode == MessagingErrorCode.Unregistered)
                {
                    // Token is dead (app uninstalled / token expired) — clear it so we stop
                    // wasting calls on it for every future notification.
                    _logger.LogWarning(
                        "FCM token for user {UserId} is unregistered. Clearing stored token.", userId);
                    await _userRepo.ClearFcmTokenAsync(userId, user.FcmToken);
                }
                catch (FirebaseMessagingException ex) when (ex.MessagingErrorCode == MessagingErrorCode.InvalidArgument)
                {
                    // Can mean a bad token OR a bad payload (e.g. invalid channel/data value).
                    // Don't clear the token automatically — that could wipe valid tokens because
                    // of a payload bug. Log it so it can be investigated.
                    _logger.LogError(ex,
                        "FCM rejected push for user {UserId} with InvalidArgument. Token kept; check payload/token.",
                        userId);
                }
                catch (FirebaseMessagingException ex)
                {
                    // Transient failure (rate limit, server error, network). Token may still be valid.
                    _logger.LogError(ex,
                        "Transient FCM error sending push to user {UserId} ({ErrorCode}). Token kept.",
                        userId, ex.MessagingErrorCode);
                }
            }
            catch (OperationCanceledException)
            {
                throw; // let cancellation propagate
            }
            catch (Exception ex)
            {
                // Never let a push failure fail the notification-creation flow that already succeeded.
                _logger.LogError(ex, "Unexpected error sending push notification to user {UserId}.", userId);
            }
        }
    }
}