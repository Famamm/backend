namespace fleetops_backend.Application.DTOs
{   
    public class LoginResponseDTO
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        public required UserInfoDTO User { get; set; }
    }
}