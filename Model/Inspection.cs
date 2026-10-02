using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public class Inspection
    {
        public int Id { get; set; }

        [Column("driver_id")]
        public int DriverId { get; set; }
        public Driver? Driver { get; set; }

        [Column("vehicle_id")]
        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        [Column("organization_id")]
        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public bool Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        
        public ICollection<InspectionResult> Results { get; set; } = new List<InspectionResult>();
        public ICollection<InspectionPhoto> Photos { get; set; } = new List<InspectionPhoto>();
    }
}