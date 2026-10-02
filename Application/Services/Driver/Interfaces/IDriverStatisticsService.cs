using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IDriverStatisticsService
    {
        Task<DriverStatisticsDto> GetStatisticsAsync(int driverId);
        Task<DriverStatisticsDto> GetStatisticsByUserIdAsync(int userId);
    }
}
