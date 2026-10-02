using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public class DamageReport
    {
        public int Id { get; set; }

        [Column("trip_id")]
        public int TripId { get; set; }

        [ForeignKey(nameof(TripId))]
        public Trip? Trip { get; set; }

        [Column("driver_id")]
        public int DriverId { get; set; }

        [ForeignKey(nameof(DriverId))]
        public Driver? Driver { get; set; }

        [Column("vehicle_id")]
        public int VehicleId { get; set; }

        [ForeignKey(nameof(VehicleId))]
        public Vehicle? Vehicle { get; set; }

        [Column("organization_id")]
        public int OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public Organization? Organization { get; set; }

        public DamageSeverity Severity { get; set; }

        // 1 = paling mendesak, 3 = paling rendah. Diturunkan dari Severity + OperationalStatus.
        public int Priority { get; set; }

        public OperationalStatus OperationalStatus { get; set; }

        public DamageReportStatus Status { get; set; } = DamageReportStatus.Reported;

        public string? DamageType { get; set; }

        public string? Note { get; set; }

        [Column("incident_at")]
        public DateTimeOffset IncidentAt { get; set; }

        public string? Location { get; set; }

        public int? Odometer { get; set; }

        [Column("image_url_path")]
        public string? ImageUrlPath { get; set; }

        public string? ReviewNote { get; set; }

        [Column("resolved_at")]
        public DateTimeOffset? ResolvedAt { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
