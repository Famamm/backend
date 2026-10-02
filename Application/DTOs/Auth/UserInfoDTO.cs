namespace fleetops_backend.Application.DTOs
{
    public class UserInfoDTO
    {
        public int UserId { get; set; }
        public int? DriverId { get; set; }
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }
}