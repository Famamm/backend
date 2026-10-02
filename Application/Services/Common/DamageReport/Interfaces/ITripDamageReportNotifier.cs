using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface ITripDamageReportNotifier
    {
        Task NotifyNewReportAsync(DamageReport report);
        Task NotifyStatusChangedAsync(DamageReport report);
    }
}
