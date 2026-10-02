using fleetops_backend.Application.DTOs;
using Microsoft.Extensions.Options;

namespace fleetops_backend.Application.Services
{
    public class FuelBaselineCalculator : IFuelBaselineCalculator
    {
        private readonly FuelAnomalyOptions _options;

        public FuelBaselineCalculator(IOptions<FuelAnomalyOptions> options)
        {
            _options = options.Value;
        }

        public FuelBaselineDTO Calculate(IReadOnlyList<FuelFill> fills)
        {
            var efficiencies = GetSegmentEfficiencies(fills)
                .TakeLast(_options.Window)
                .ToList();

            return new FuelBaselineDTO
            {
                AverageKmPerLiter = efficiencies.Count > 0 ? Math.Round(efficiencies.Average(), 2) : null,
                SegmentCount = efficiencies.Count,
                IsReliable = efficiencies.Count >= _options.MinSegments
            };
        }

        private static IEnumerable<double> GetSegmentEfficiencies(IReadOnlyList<FuelFill> fills)
        {
            return fills
                .Zip(fills.Skip(1), (previous, current) => current.KmPerLiterSince(previous))
                .OfType<double>();
        }
    }
}
