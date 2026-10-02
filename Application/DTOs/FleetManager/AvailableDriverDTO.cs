namespace fleetops_backend.Application.DTOs
{
    public class AvailableDriverDTO
    {
        public int DriverId { get; set; }
        public int UserId { get; set; }
        public string NamaDriver { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? ProfilePhoto { get; set; }
        public string? JenisLisensi { get; set; }
        public DateTimeOffset? TanggalBerlaku { get; set; }
        public string? Status { get; set; }
    }
}
