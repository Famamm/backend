using NetTopologySuite.Geometries;
using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public class TripGpsPoint
    {
        public int Id { get; set; }

        [Column("trip_id")]
        public int TripId { get; set; }

        [ForeignKey(nameof(TripId))]
        public Trip? Trip { get; set; }

        public Guid ClientId { get; set; }

        [Column("coordinate", TypeName = "geography")]
        public Point Coordinate { get; set; } = default!;

        [Column("captured_at")]
        public DateTimeOffset CapturedAt { get; set; }

        [Column("accuracy_meters")]
        public double? AccuracyMeters { get; set; }
    }
}