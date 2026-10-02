using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class InspectionDamageNotifier : IInspectionDamageNotifier
    {
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;
        private readonly ILogger<InspectionDamageNotifier> _logger;

        public InspectionDamageNotifier(
            AppDbContext context,
            INotificationService notificationService,
            ILogger<InspectionDamageNotifier> logger)
        {
            _context = context;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task NotifyNewDamageAsync(Inspection inspection, IReadOnlyCollection<int> previousNotOkItemIds)
        {
            try
            {
                var newDamageCount = InspectionDamageDetector
                    .FindNewNotOkItemIds(previousNotOkItemIds, inspection.Results)
                    .Count;

                if (newDamageCount == 0)
                {
                    return;
                }

                var driverName = await _context.Drivers
                    .Where(d => d.Id == inspection.DriverId)
                    .Select(d => d.User!.FullName)
                    .FirstAsync();

                var plateNumber = await _context.Vehicles
                    .Where(v => v.Id == inspection.VehicleId)
                    .Select(v => v.PlatNomor)
                    .FirstAsync();

                var content = NotificationMessageBuilder.BuildDamageReport(driverName, plateNumber, inspection.Id, newDamageCount);
                await _notificationService.NotifyFleetManagersAsync(inspection.OrganizationId, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal mengirim notifikasi laporan kerusakan untuk inspeksi {InspectionId}", inspection.Id);
            }
        }
    }
}
