namespace fleetops_backend.Application.DTOs
{
    public class SaveTripGpsDTO
    {
        public List<TripGpsPointDTO> Points { get; set; } = new();
    }

    public class SaveTripGpsResultDTO
    {
        public int ReceivedCount { get; set; }
        public int SavedCount { get; set; }
        public int DuplicateCount { get; set; }
    }

    public class TripGpsPointDTO
    {
        public Guid ClientId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTimeOffset CapturedAt { get; set; }
        public double? AccuracyMeters { get; set; }
    }
}