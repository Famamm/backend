using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IFleetManagerVehicleService
    {
        Task<FleetDashboardDTO> GetSummaryAsync(int organizationId);
        Task<List<FleetVehicleDTO>> GetVehiclesAsync(int organizationId);
    }
}
