namespace fleetops_backend.Application.DTOs
{
    public class UserListItemDTO
    {
        public int UserId { get; set; }
        public int? DriverId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfilePhoto { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Status { get; set; }
    }
}
