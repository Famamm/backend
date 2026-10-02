namespace fleetops_backend.Application.DTOs
{
    public class DamageReportDTO
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public int VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public string? PlatNomor { get; set; }
        public int DriverId { get; set; }
        public string? DriverName { get; set; }

        public required string Severity { get; set; }
        public int Priority { get; set; }
        public required string OperationalStatus { get; set; }
        public required string Status { get; set; }

        public string? DamageType { get; set; }
        public string? Note { get; set; }
        public DateTimeOffset IncidentAt { get; set; }
        public string? Location { get; set; }
        public int? Odometer { get; set; }
        public string? ImageUrlPath { get; set; }
        public string? ReviewNote { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
