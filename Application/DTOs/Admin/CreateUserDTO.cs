namespace fleetops_backend.Application.DTOs
{
    public class CreateUserDTO
    {
        public required string FullName { get; set; }
        public string? Address { get; set; }
        public IFormFile? ProfilePhotoFile { get; set; }
    }
}
