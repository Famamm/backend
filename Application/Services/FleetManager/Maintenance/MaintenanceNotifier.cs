using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class MaintenanceNotifier : IMaintenanceNotifier
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly ILogger<MaintenanceNotifier> _logger;

        public MaintenanceNotifier(
            AppDbContext context,
            INotificationService notificationService,
            ILogger<MaintenanceNotifier> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public Task NotifyReminderAsync(Maintenance maintenance, MaintenanceDueInfo due)
        {
            return SendAsync(maintenance, plate =>
                NotificationMessageBuilder.BuildMaintenanceReminder(plate, maintenance.Jenis, maintenance.Id, due));
        }

        public Task NotifyOverdueAsync(Maintenance maintenance)
        {
            return SendAsync(maintenance, plate =>
                NotificationMessageBuilder.BuildMaintenanceOverdue(plate, maintenance.Jenis, maintenance.Id));
        }

        public Task NotifyStatusChangedAsync(Maintenance maintenance)
        {
            return SendAsync(maintenance, plate =>
                NotificationMessageBuilder.BuildMaintenanceStatusChanged(
                    plate, maintenance.Jenis, maintenance.Id, maintenance.Status));
        }

        private async Task SendAsync(Maintenance maintenance, Func<string, NotificationContent> buildContent)
        {
            try
            {
                var vehicle = await _context.Vehicles
                    .Where(v => v.Id == maintenance.VehicleId)
                    .Select(v => new
                    {
                        v.PlatNomor,
                        DriverUserId = v.Driver != null ? (int?)v.Driver.UserId : null
                    })
                    .FirstAsync();

                var driverRecipients = vehicle.DriverUserId is { } driverUserId ? new[] { driverUserId } : [];

                await _notificationService.NotifyFleetManagersAsync(
                    maintenance.OrganizationId,
                    buildContent(vehicle.PlatNomor),
                    driverRecipients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal mengirim notifikasi pemeliharaan {MaintenanceId}", maintenance.Id);
            }
        }
    }
}
