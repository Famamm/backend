namespace fleetops_backend.Application.Services
{
    public interface IMaintenanceReminderService
    {
        Task ScanAsync(CancellationToken cancellationToken = default);
        Task CheckVehicleAsync(int vehicleId);
    }
}
