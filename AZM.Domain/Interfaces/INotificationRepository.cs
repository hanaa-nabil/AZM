using AZM.Domain.Entities;
using AZM.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Domain.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken ct = default);
        Task<List<Notification>> GetForUserAsync(Guid userId, int page, int pageSize, CancellationToken ct = default);
        Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default);
        Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken ct = default);
        Task DeleteAllAsync(Guid userId);
        Task<bool> ExistsForDateAsync(Guid userId, NotificationType type, DateOnly date);
        Task MarkAllReadAsync(Guid userId);
    }
}
