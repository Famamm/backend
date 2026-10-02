namespace fleetops_backend.Application.DTOs
{
    public class MaintenanceScheduleDTO
    {
        public required string Jenis { get; set; }
        public DateOnly? TanggalJadwal { get; set; }
        public int? TargetOdometer { get; set; }
        public string? Catatan { get; set; }
    }
}
