namespace fleetops_backend.Application.Services
{
    public class FuelAnomalyOptions
    {
        public const string SectionName = "FuelAnomaly";

        public int Window { get; set; } = 5;
        public int MinSegments { get; set; } = 3;
        public double DeviationThreshold { get; set; } = 0.3;
    }
}
