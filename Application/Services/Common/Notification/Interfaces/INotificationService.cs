namespace fleetops_backend.Application.Services
{
    public interface INotificationService
    {
        Task NotifyFleetManagersAsync(
            int organizationId,
            NotificationContent content,
            IReadOnlyCollection<int>? additionalUserIds = null);

        Task NotifyAdminsAsync(
            int organizationId,
            NotificationContent content,
            IReadOnlyCollection<int>? additionalUserIds = null);

        Task NotifyAdminsAndFleetManagersAsync(
            int organizationId,
            NotificationContent content,
            IReadOnlyCollection<int>? additionalUserIds = null);

        Task NotifyUsersAsync(IReadOnlyCollection<int> userIds, NotificationContent content);
    }
}
