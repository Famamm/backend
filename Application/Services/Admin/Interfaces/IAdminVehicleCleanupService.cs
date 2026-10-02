using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IAdminVehicleCleanupService
    {
        Task DeleteVehicleDependenciesAsync(int vehicleId);
    }
}
