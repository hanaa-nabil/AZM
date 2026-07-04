using AZM.Domain.Enums;

namespace AZM.Domain.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(Guid recipientId, NotificationType type, string title, string body,
            Guid? relatedEventId = null, CancellationToken ct = default);

        Task SendBulkAsync(IEnumerable<Guid> recipientIds, NotificationType type, string title, string body,
            Guid? relatedEventId = null, CancellationToken ct = default);

        Task SendToGroupAsync(IEnumerable<string> userIds, string title, string body, CancellationToken ct = default);
    }

}