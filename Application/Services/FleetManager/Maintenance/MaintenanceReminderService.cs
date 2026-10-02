using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace fleetops_backend.Application.Services
{
    public class MaintenanceReminderService : IMaintenanceReminderService
    {
        private readonly AppDbContext _context;
        private readonly IMaintenanceNotifier _notifier;
        private readonly MaintenanceReminderOptions _options;
        private readonly ILogger<MaintenanceReminderService> _logger;

        public MaintenanceReminderService(
            AppDbContext context,
            IMaintenanceNotifier notifier,
            IOptions<MaintenanceReminderOptions> options,
            ILogger<MaintenanceReminderService> logger)
        {
            _context = context;
            _notifier = notifier;
            _options = options.Value;
            _logger = logger;
        }

        public Task ScanAsync(CancellationToken cancellationToken = default)
        {
            return ProcessAsync(vehicleId: null, cancellationToken);
        }

        public async Task CheckVehicleAsync(int vehicleId)
        {
            try
            {
                await ProcessAsync(vehicleId, CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal memeriksa pengingat pemeliharaan untuk kendaraan {VehicleId}", vehicleId);
            }
        }

        private async Task ProcessAsync(int? vehicleId, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var pending = await _context.Maintenances
                .AsNoTracking()
                .Include(m => m.Vehicle)
                .Where(m => m.Status == MaintenanceStatus.Scheduled
                    && (m.OverdueNotifiedAt == null || m.ReminderStage < _options.MaxStage)
                    && (vehicleId == null || m.VehicleId == vehicleId))
                .ToListAsync(cancellationToken);

            foreach (var maintenance in pending)
            {
                var due = MaintenanceDueCalculator.Evaluate(maintenance, maintenance.Vehicle!.Odometer, today, _options);

                if (due?.State == MaintenanceDueState.Overdue && maintenance.OverdueNotifiedAt == null)
                {
                    if (await ClaimOverdueAsync(maintenance.Id, cancellationToken))
                    {
                        await _notifier.NotifyOverdueAsync(maintenance);
                    }
                    continue;
                }

                var targetStage = due is null
                    ? 0
                    : MaintenanceDueCalculator.GetReminderStage(due, _options);

                if (targetStage > maintenance.ReminderStage
                    && await ClaimReminderAsync(maintenance.Id, targetStage, cancellationToken))
                {
                    await _notifier.NotifyReminderAsync(maintenance, due!);
                }
            }
        }

        // Klaim atomik per tingkat lewat UPDATE bersyarat supaya dua pemicu bersamaan (worker dan akhir trip)
        // tidak mengirim notifikasi ganda untuk tingkat yang sama.
        private async Task<bool> ClaimReminderAsync(
            int maintenanceId, int targetStage, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;

            var updated = await _context.Maintenances
                .Where(m => m.Id == maintenanceId
                    && m.Status == MaintenanceStatus.Scheduled
                    && m.ReminderStage < targetStage)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(m => m.ReminderStage, targetStage)
                    .SetProperty(m => m.ReminderSentAt, now), cancellationToken);

            return updated == 1;
        }

        private async Task<bool> ClaimOverdueAsync(int maintenanceId, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.UtcNow;

            var updated = await _context.Maintenances
                .Where(m => m.Id == maintenanceId
                    && m.Status == MaintenanceStatus.Scheduled
                    && m.OverdueNotifiedAt == null)
                .ExecuteUpdateAsync(
                    s => s
                        .SetProperty(m => m.OverdueNotifiedAt, now)
                        .SetProperty(m => m.ReminderSentAt, m => m.ReminderSentAt ?? now)
                        .SetProperty(m => m.ReminderStage, _options.MaxStage),
                    cancellationToken);

            return updated == 1;
        }
    }
}
