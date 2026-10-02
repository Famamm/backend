namespace fleetops_backend.Application.DTOs
{
    public class LoginDTO
    {
        public required string Identity { get; set; }
        public required string Password { get; set; }
    }
}