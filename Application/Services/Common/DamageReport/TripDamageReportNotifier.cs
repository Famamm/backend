using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class TripDamageReportNotifier : ITripDamageReportNotifier
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly ILogger<TripDamageReportNotifier> _logger;

        public TripDamageReportNotifier(
            AppDbContext context,
            INotificationService notificationService,
            ILogger<TripDamageReportNotifier> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task NotifyNewReportAsync(DamageReport report)
        {
            try
            {
                var driverName = await _context.Drivers
                    .Where(d => d.Id == report.DriverId)
                    .Select(d => d.User!.FullName)
                    .FirstAsync();

                var plateNumber = await _context.Vehicles
                    .Where(v => v.Id == report.VehicleId)
                    .Select(v => v.PlatNomor)
                    .FirstAsync();

                var content = NotificationMessageBuilder.BuildTripDamageReport(
                    driverName,
                    plateNumber,
                    report.Id,
                    report.Severity,
                    report.OperationalStatus);

                await _notificationService.NotifyAdminsAndFleetManagersAsync(report.OrganizationId, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal mengirim notifikasi laporan kerusakan trip {DamageReportId}", report.Id);
            }
        }

        public async Task NotifyStatusChangedAsync(DamageReport report)
        {
            try
            {
                var plateNumber = await _context.Vehicles
                    .Where(v => v.Id == report.VehicleId)
                    .Select(v => v.PlatNomor)
                    .FirstAsync();

                var content = NotificationMessageBuilder.BuildTripDamageReportStatusChanged(
                    plateNumber,
                    report.Id,
                    report.Status);

                // Beri tahu driver pelapor + admin & fleet manager.
                var driverUserId = await _context.Drivers
                    .Where(d => d.Id == report.DriverId)
                    .Select(d => d.UserId)
                    .FirstAsync();

                await _notificationService.NotifyAdminsAndFleetManagersAsync(
                    report.OrganizationId,
                    content,
                    [driverUserId]);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal mengirim notifikasi perubahan status laporan kerusakan {DamageReportId}", report.Id);
            }
        }
    }
}
