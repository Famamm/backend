using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IDriverDashboardService
    {
        Task<DriverDashboardDTO> GetDashboardDriverAsync(int driverId);
        Task<DriverDashboardDTO> GetDashboardDriverByUserIdAsync(int userId);
    }
}
