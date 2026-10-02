namespace fleetops_backend.Application.DTOs
{
    public class AccountInformationDTO
    {
        public string? PhotoUrl { get; set; }
        public required string FullName { get; set; }
        public required string Email { get; set; }  
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class AccountDTO
    {
        public required AccountInformationDTO Information { get; set; }
        public required string UserName { get; set; }
        public required string Role { get; set; }
    }
}