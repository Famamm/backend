using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IRefreshTokenService
    {
        Task<string> CreateAsync(int userId);

        Task<RefreshToken?> FindValidAsync(string rawToken);

        Task<(RefreshToken Entity, string RawToken)> RotateAsync(
            RefreshToken currentToken,
            int userId);

        Task<bool> RevokeAsync(string rawToken);

        Task<bool> RevokeAllAsync(int userId);
    }
}