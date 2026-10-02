namespace fleetops_backend.Application.DTOs
{
    public class UpdateDriverDTO
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public IFormFile? ProfilePhotoFile { get; set; }
        public string? JenisLisensi { get; set; }
        public IFormFile? FotoLisensiFile { get; set; }
        public DateTimeOffset? TanggalBerlaku { get; set; }
        public string? Status { get; set; }
    }
}
