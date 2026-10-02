using System.ComponentModel.DataAnnotations; // <-- Tambahkan ini
using System.ComponentModel.DataAnnotations.Schema;
using NetTopologySuite.Geometries;

namespace fleetops_backend.Models
{
    public class Fuel
    {
        [Key]
        public int Id { get; set; }

        [Column("vehicle_id")]
        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        [Column("driver_id")]
        public int DriverId { get; set; }
        public Driver? Driver { get; set; }

        [Column("organization_id")]
        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        [Column("fuel_price_id")]
        public int FuelPriceId { get; set; }
        public FuelPrice? FuelPrice { get; set; }

        public decimal JmlLiter { get; set; }
        public int HargaTotal { get; set; }
        public Point? LokasiPengisian { get; set; }
        public int OdometerIsi {get; set; }
        public string? ImageUrlPath { get; set; }
        public string? Catatan {get; set;}
        public DateTime CreatedAt { get; set; }
    } 
}

