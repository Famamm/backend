using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public class FuelPrice
    {
        [Key]
        public int Id { get; set; }

        [Column("fuel_type")]
        public string FuelType { get; set; } = string.Empty; // Pertalite, BioSolar, Pertamax, etc

        [Column("price_per_liter")]
        public int PricePerLiter { get; set; } // Harga per liter dalam Rupiah

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        // Relationships
        public ICollection<Fuel>? Fuels { get; set; }
    }
}