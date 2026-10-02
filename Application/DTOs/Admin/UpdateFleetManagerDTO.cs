namespace fleetops_backend.Application.DTOs
{
    // Fleet manager tidak punya field tambahan di luar data user dasar,
    // jadi update hanya menyentuh data user.
    public class UpdateFleetManagerDTO
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Status { get; set; }
        public IFormFile? ProfilePhotoFile { get; set; }
    }
}
