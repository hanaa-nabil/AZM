using AZM.Domain.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AZM.Infrastructure.Services
{
    public class FcmNotificationService : INotificationService
    {
        private readonly FirebaseMessaging _messaging;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<FcmNotificationService> _logger;

        public FcmNotificationService(
            IOptions<FcmOptions> options,
            IUserRepository userRepo,
            ILogger<FcmNotificationService> logger)
        {
            _userRepo = userRepo;
            _logger = logger;

            // Initialize Firebase only once
            if (FirebaseApp.DefaultInstance is null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile(options.Value.ServiceAccountKeyPath)
                });
            }

            _messaging = FirebaseMessaging.DefaultInstance;
        }

        // Send to a single user
        public async Task SendToUserAsync(string userId, string title, string body)
        {
            // Get the user's FCM device token from DB
            var user = await _userRepo.GetByIdAsync(userId);

            if (user is null || string.IsNullOrEmpty(user.FcmToken))
            {
                _logger.LogWarning("No FCM token found for user {UserId}", userId);
                return;
            }

            var message = new Message
            {
                Token = user.FcmToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                Android = new AndroidConfig
                {
                    Priority = Priority.High,
                    Notification = new AndroidNotification
                    {
                        Title = title,
                        Body = body,
                        Sound = "default"
                    }
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        Alert = new ApsAlert { Title = title, Body = body },
                        Sound = "default"
                    }
                }
            };

            try
            {
                var result = await _messaging.SendAsync(message);
                _logger.LogInformation("FCM sent to {UserId}, messageId: {MessageId}", userId, result);
            }
            catch (FirebaseMessagingException ex)
            {
                _logger.LogError(ex, "FCM failed for user {UserId}", userId);
            }
        }

        // Send to multiple users at once (SOS alert, event reminder, etc.)
        public async Task SendToGroupAsync(IEnumerable<string> userIds, string title, string body)
        {
            // Get all FCM tokens for these users
            var tokens = new List<string>();

            foreach (var userId in userIds)
            {
                var user = await _userRepo.GetByIdAsync(userId);
                if (user is not null && !string.IsNullOrEmpty(user.FcmToken))
                    tokens.Add(user.FcmToken);
            }

            if (tokens.Count == 0)
            {
                _logger.LogWarning("No FCM tokens found for group notification");
                return;
            }

            // Firebase allows max 500 tokens per batch
            var batches = tokens.Chunk(500);

            foreach (var batch in batches)
            {
                var multicastMessage = new MulticastMessage
                {
                    Tokens = batch.ToList(),
                    Notification = new Notification
                    {
                        Title = title,
                        Body = body
                    },
                    Android = new AndroidConfig
                    {
                        Priority = Priority.High,
                        Notification = new AndroidNotification
                        {
                            Title = title,
                            Body = body,
                            Sound = "default"
                        }
                    },
                    Apns = new ApnsConfig
                    {
                        Aps = new Aps
                        {
                            Alert = new ApsAlert { Title = title, Body = body },
                            Sound = "default"
                        }
                    }
                };

                try
                {
                    var result = await _messaging.SendEachForMulticastAsync(multicastMessage);
                    _logger.LogInformation(
                        "FCM group sent: {Success} success, {Failure} failed",
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
