namespace fleetops_backend.Application.DTOs.Admin
{
    public class DriverResponseDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? JenisLisensi { get; set; }
        public string? FotoLisensi { get; set; }
        public DateTimeOffset? TanggalBerlaku { get; set; }
        public string? Status { get; set; }
    }
}
