using AZM.Domain.Entities;
using AZM.Domain.Interfaces;
using AZM.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AZM.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(Notification notification, CancellationToken ct = default)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync(ct);
        }

        public async Task AddRangeAsync(IEnumerable<Notification> notifications, CancellationToken ct = default)
        {
            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync(ct);
        }

        public Task<List<Notification>> GetForUserAsync(Guid userId, int page, int pageSize, CancellationToken ct = default)
            => _context.Notifications
                .Where(n => n.RecipientId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

        public Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
            => _context.Notifications.CountAsync(n => n.RecipientId == userId && !n.IsRead, ct);

        public async Task MarkAsReadAsync(Guid notificationId, Guid userId, CancellationToken ct = default)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.RecipientId == userId, ct);
            if (notification is null) return;

            notification.IsRead = true;
            await _context.SaveChangesAsync(ct);
        }

        public async Task MarkAllAsReadAsync(Guid userId, CancellationToken ct = default)
        {
            await _context.Notifications
                .Where(n => n.RecipientId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true), ct);
        }
    }
}
