using Microsoft.Extensions.Options;

namespace fleetops_backend.Application.Services
{
    public class FuelAnomalyDetector : IFuelAnomalyDetector
    {
        private readonly IFuelBaselineCalculator _baselineCalculator;
        private readonly FuelAnomalyOptions _options;

        public FuelAnomalyDetector(
            IFuelBaselineCalculator baselineCalculator,
            IOptions<FuelAnomalyOptions> options)
        {
            _baselineCalculator = baselineCalculator;
            _options = options.Value;
        }

        public FuelAnomaly? Detect(IReadOnlyList<FuelFill> fills)
        {
            if (fills.Count < 2)
            {
                return null;
            }

            var baseline = _baselineCalculator.Calculate(fills.Take(fills.Count - 1).ToList());
            var kmPerLiter = fills[^1].KmPerLiterSince(fills[^2]);

            if (!baseline.IsReliable || kmPerLiter == null)
            {
                return null;
            }

            var anomaly = new FuelAnomaly(kmPerLiter.Value, baseline.AverageKmPerLiter!.Value);

            return Math.Abs(anomaly.Deviation) > _options.DeviationThreshold ? anomaly : null;
        }
    }
}
