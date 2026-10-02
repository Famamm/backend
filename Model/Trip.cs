using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public class Trip
    {
        public int Id { get; set; }

        [Column("driver_id")]
        public int DriverId { get; set; }
        
        [ForeignKey(nameof(DriverId))]
        public Driver? Driver { get; set; }

        [Column("vehicle_id")]
        public int VehicleId { get; set; }
        
        [ForeignKey(nameof(VehicleId))]
        public Vehicle? Vehicle { get; set; }

        [Column("kord_awal", TypeName = "geography")]
        public Point? KordAwal { get; set; }

        [Column("kord_akhir", TypeName = "geography")]
        public Point? KordAkhir { get; set; }
        public string? LokasiAwal {get; set;}
        public string? LokasiAkhir {get; set;}

        [Column("odometer_awal")]
        public int? OdometerAwal { get; set; }

        [Column("odometer_akhir")]
        public int? OdometerAkhir { get; set; }

        [Column("waktu_mulai")]
        public DateTimeOffset WaktuMulai { get; set; }

        [Column("waktu_selesai")]
        public DateTimeOffset? WaktuSelesai { get; set; }

        public string? Catatan { get; set; }

        public string Status { get; set; } = string.Empty;

        [Column("organization_id")]
        public required int OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public Organization? Organization { get; set; }

        public ICollection<TripDetail> Detail { get; set; } = new List<TripDetail>();
        public ICollection<TripGpsPoint> GpsPoints { get; set; }
        = new List<TripGpsPoint>();
        public ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();
    }
}