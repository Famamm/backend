using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IMaintenanceNotifier
    {
        Task NotifyReminderAsync(Maintenance maintenance, MaintenanceDueInfo due);
        Task NotifyOverdueAsync(Maintenance maintenance);
        Task NotifyStatusChangedAsync(Maintenance maintenance);
    }
}
