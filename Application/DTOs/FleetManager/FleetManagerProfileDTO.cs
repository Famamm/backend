using Microsoft.AspNetCore.Http;

namespace fleetops_backend.Application.DTOs
{
    public class FleetManagerProfileDTO
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public IFormFile? ProfilePhoto { get; set; }
    }
}
