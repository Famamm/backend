using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IUserNotificationService
    {
        Task<List<NotificationDTO>> GetNotificationsAsync(int userId, bool unreadOnly);
        Task<NotificationDTO> MarkAsReadAsync(int userId, int notificationId);
        Task<int> MarkAllAsReadAsync(int userId);
    }
}
