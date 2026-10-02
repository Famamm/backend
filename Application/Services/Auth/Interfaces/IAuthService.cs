using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDTO?> LoginAsync(string identity, string password);
        Task<bool?> RegisterAsync(string organizationName, string fullName, string email, string password);
        Task<RefreshTokenResponseDTO?> RefreshAccessTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> LogoutAllDevicesAsync(int userId);
    }
}

