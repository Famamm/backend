namespace fleetops_backend.Application.DTOs
{    
    public class RegisterOrganizationDTO
    {
        public required string OrganizationName { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}