namespace fleetops_backend.Application.DTOs
{
    public class UpdateMaintenanceStatusDTO
    {
        public required string Status { get; set; }
        public int? Odometer { get; set; }
        public decimal? Biaya { get; set; }
        public string? Catatan { get; set; }
    }
}
