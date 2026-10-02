using Microsoft.AspNetCore.Http;

namespace fleetops_backend.Application.DTOs.Admin
{
    public class UpdateAdminProfileDTO
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public IFormFile? ProfilePhoto { get; set; }
    }
}