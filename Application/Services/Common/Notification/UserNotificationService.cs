using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class UserNotificationService : IUserNotificationService
    {
        private readonly AppDbContext _context;

        public UserNotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<NotificationDTO>> GetNotificationsAsync(int userId, bool unreadOnly)
        {
            var query = _context.Notifications.Where(n => n.UserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return notifications.Select(ToDto).ToList();
        }

        public async Task<NotificationDTO> MarkAsReadAsync(int userId, int notificationId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId)
                ?? throw new KeyNotFoundException("Notifikasi tidak ditemukan.");

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return ToDto(notification);
        }

        public Task<int> MarkAllAsReadAsync(int userId)
        {
            return _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(setters => setters.SetProperty(n => n.IsRead, true));
        }

        private static NotificationDTO ToDto(Notification notification)
        {
            return new NotificationDTO
            {
                Id = notification.Id,
                Type = notification.Type.ToString(),
                Title = notification.Title,
                Message = notification.Message,
                ReferenceId = notification.ReferenceId,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }
    }
}
