using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public class TripDetail
    {
        public int Id { get; set; }

        [Column("trip_id")]
        public required int TripId { get; set; }

        [ForeignKey(nameof(TripId))]
        public Trip? Trip { get; set; }

        public int Jarak { get; set; }
        public int AvgSpeed { get; set; }
        public int Waktu { get; set; }
        public string? ImageUrl { get; set; }
    }
}