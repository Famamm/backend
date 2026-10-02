using fleetops_backend.Models;
using Microsoft.Extensions.Options;

namespace fleetops_backend.Application.Services
{
    public class FuelAnomalyService : IFuelAnomalyService
    {
        private readonly IFuelFillReader _fillReader;
        private readonly IFuelAnomalyDetector _detector;
        private readonly INotificationService _notificationService;
        private readonly FuelAnomalyOptions _options;
        private readonly ILogger<FuelAnomalyService> _logger;

        public FuelAnomalyService(
            IFuelFillReader fillReader,
            IFuelAnomalyDetector detector,
            INotificationService notificationService,
            IOptions<FuelAnomalyOptions> options,
            ILogger<FuelAnomalyService> logger)
        {
            _fillReader = fillReader;
            _detector = detector;
            _notificationService = notificationService;
            _options = options.Value;
            _logger = logger;
        }

        public async Task CheckAsync(Vehicle vehicle, Fuel fuel)
        {
            try
            {
                var fills = await _fillReader.GetLatestAsync(vehicle.Id, _options.Window + 2);
                var anomaly = _detector.Detect(fills);

                if (anomaly == null)
                {
                    return;
                }

                var content = NotificationMessageBuilder.BuildFuelAnomaly(vehicle.PlatNomor, fuel.Id, anomaly);
                await _notificationService.NotifyFleetManagersAsync(vehicle.OrganizationId, content);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gagal memeriksa anomali BBM untuk fuel {FuelId}", fuel.Id);
            }
        }
    }
}
