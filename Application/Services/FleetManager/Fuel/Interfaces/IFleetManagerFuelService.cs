using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IFleetManagerFuelService
    {
        Task<FuelBaselineDTO> GetVehicleBaselineAsync(int organizationId, int vehicleId);
    }
}
