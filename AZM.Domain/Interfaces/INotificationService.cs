namespace AZM.Domain.Interfaces
{
    public interface INotificationService
    {
        Task SendToUserAsync(string userId, string title, string body);
        Task SendToGroupAsync(IEnumerable<string> userIds, string title, string body);
    }

}