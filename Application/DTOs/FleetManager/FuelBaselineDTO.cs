namespace fleetops_backend.Application.DTOs
{
    public class FuelBaselineDTO
    {
        public double? AverageKmPerLiter { get; set; }
        public int SegmentCount { get; set; }
        public bool IsReliable { get; set; }
    }
}
