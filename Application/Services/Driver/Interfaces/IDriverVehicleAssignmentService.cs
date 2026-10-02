using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IDriverVehicleAssignmentService
    {
        Task<List<DriverVehicleDto>> GetAvailableVehiclesAsync(int organizationId, int driverId);
    }
}
