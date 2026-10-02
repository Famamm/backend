using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IFleetManagerStatisticsService
    {
        Task<FleetStatisticsDTO> GetStatisticsAsync(int organizationId, int? bulan, int? tahun);
    }
}
