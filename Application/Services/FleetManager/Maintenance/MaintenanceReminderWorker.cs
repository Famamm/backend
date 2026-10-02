using Microsoft.Extensions.Options;

namespace fleetops_backend.Application.Services
{
    public class MaintenanceReminderWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly MaintenanceReminderOptions _options;
        private readonly ILogger<MaintenanceReminderWorker> _logger;

        public MaintenanceReminderWorker(
            IServiceScopeFactory scopeFactory,
            IOptions<MaintenanceReminderOptions> options,
            ILogger<MaintenanceReminderWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _options = options.Value;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMinutes(Math.Max(1, _options.ScanIntervalMinutes)));

            do
            {
                await ScanAsync(stoppingToken);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task ScanAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var reminderService = scope.ServiceProvider.GetRequiredService<IMaintenanceReminderService>();
                await reminderService.ScanAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Pemindaian pengingat pemeliharaan gagal");
            }
        }
    }
}
