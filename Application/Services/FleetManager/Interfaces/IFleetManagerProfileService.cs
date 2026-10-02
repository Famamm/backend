using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IFleetManagerProfileService
    {
        Task<AccountDTO> GetProfileAsync(int userId);
        Task<AccountDTO> UpdateProfileAsync(int userId, FleetManagerProfileDTO dto);
    }
}
