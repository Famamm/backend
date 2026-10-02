using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IDriverMaintenanceService
    {
        Task<DriverMaintenanceStatusDTO> GetMaintenanceStatusAsync(int userId);
    }
}
