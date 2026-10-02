using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        int GetRefreshTokenExpiryDays();
        int GetAccessTokenExpiryMinutes();
    }
}

