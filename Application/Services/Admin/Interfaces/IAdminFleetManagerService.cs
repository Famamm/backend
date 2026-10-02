using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IAdminFleetManagerService
    {
        Task<(User user, string generatedPassword, string username)> CreateFleetManagerAsync(
            int organizationId,
            CreateFleetManagerDTO dto);

        Task<List<UserListItemDTO>> GetFleetManagersAsync(int organizationId);

        Task<FleetManagerDetailDTO> GetFleetManagerByIdAsync(int organizationId, int userId);

        Task<User> UpdateFleetManagerAsync(int organizationId, int userId, UpdateFleetManagerDTO dto);

        Task DeleteFleetManagerAsync(int organizationId, int userId);

        Task<(User user, string generatedPassword)> ResetFleetManagerPasswordAsync(int organizationId, int userId);
    }
}
