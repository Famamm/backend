namespace fleetops_backend.Application.DTOs
{
    public class TripImageUploadDTO
    {
        public IFormFile Image { get; set; } = default!;
    }
}