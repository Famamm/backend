using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IDriverProfileService
    {
        Task<GetDriverProfileDTO> GetDriverProfileAsync(int driverId);
        Task<GetDriverProfileDTO> GetDriverProfileByUserIdAsync(int userId);
    }
}
