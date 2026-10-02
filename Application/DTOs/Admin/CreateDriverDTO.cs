namespace fleetops_backend.Application.DTOs
{
    public class CreateDriverDTO : CreateUserDTO
    {
        public string? JenisLisensi { get; set; }
        public IFormFile? FotoLisensiFile { get; set; }
        public DateTimeOffset? TanggalBerlaku { get; set; }
        public string? Status { get; set; }
    }
}
