namespace fleetops_backend.Application.DTOs
{
    public class MaintenanceDTO
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public required string VehicleName { get; set; }
        public required string PlatNomor { get; set; }
        public required string Jenis { get; set; }
        public required string Status { get; set; }
        public string? DueState { get; set; }
        public DateOnly? TanggalJadwal { get; set; }
        public int? TargetOdometer { get; set; }
        public int? DaysRemaining { get; set; }
        public int? KmRemaining { get; set; }
        public DateTime? Tanggal { get; set; }
        public int? Odometer { get; set; }
        public decimal? Biaya { get; set; }
        public string? Catatan { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
