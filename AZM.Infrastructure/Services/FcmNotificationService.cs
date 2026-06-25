using AZM.Domain.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace AZM.Infrastructure.Services
{
    public class FcmNotificationService : INotificationService
    {
        private readonly FirebaseMessaging? _messaging;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<FcmNotificationService> _logger;

        public FcmNotificationService(
            IUserRepository userRepo,
            ILogger<FcmNotificationService> logger)
        {
            _userRepo = userRepo;
            _logger = logger;

            // Firebase is initialized ONCE at startup in InfrastructureServiceExtensions.AddInfrastructure.
            // If that init was skipped (missing/invalid service account file), FirebaseApp.DefaultInstance
            // stays null and we degrade gracefully here instead of throwing on every resolution.
            if (FirebaseApp.DefaultInstance is not null)
            {
                _messaging = FirebaseMessaging.DefaultInstance;
            }
            else
            {
                _logger.LogWarning("Firebase not initialized — push notifications are disabled.");
                _messaging = null;
            }
        }

        public async Task SendToUserAsync(string userId, string title, string body)
        {
            if (_messaging is null) return;

            var user = await _userRepo.GetByIdAsync(userId);
            if (user is null || string.IsNullOrEmpty(user.FcmToken))
            {
                _logger.LogWarning("No FCM token for user {UserId}", userId);
                return;
            }

            var message = new Message
            {
                Token = user.FcmToken,
                Notification = new Notification { Title = title, Body = body },
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Title = title,
                        Body = body,
                        Sound = "default"
                    }
                }
            };

            try
            {
                var result = await _messaging.SendAsync(message);
                _logger.LogInformation("FCM sent to {UserId}: {Result}", userId, result);
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogError(ex, "FCM failed for user {UserId}", userId);
            }
        }

        public async Task SendToGroupAsync(IEnumerable<string> userIds, string title, string body)
        {
            if (_messaging is null) return;

            var tokens = new List<string>();
            foreach (var uid in userIds)
            {
                var user = await _userRepo.GetByIdAsync(uid);
                if (user is not null && !string.IsNullOrEmpty(user.FcmToken))
                    tokens.Add(user.FcmToken);
            }

            if (tokens.Count == 0) return;

            foreach (var batch in tokens.Chunk(500))
            {
                var multicast = new MulticastMessage
                {
                    Tokens = batch.ToList(),
                    Notification = new Notification { Title = title, Body = body }
                };

                try
                {
                    var result = await _messaging.SendEachForMulticastAsync(multicast);
                    _logger.LogInformation("FCM group: {Success} ok, {Fail} failed",
                        result.SuccessCount, result.FailureCount);
                }
                catch (FirebaseMessagingException ex)
                {
                    _logger.LogError(ex, "FCM group send failed");
                }
            }
        }
    }
}