using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IAdminVehicleService
    {
        Task<Vehicle> CreateVehicleAsync(int organizationId, VehicleDTO dto);
        Task<GetVehicleDTO> GetVehicleByIdAsync(int organizationId, int vehicleId);
        Task<Vehicle> DeleteVehicleAsync(int organizationId, int vehicleId);
        Task<Vehicle> UpdateVehicleAsync(int organizationId, int vehicleId, UpdateVehicleDTO dto);
        Task<List<GetVehicleDTO>> GetVehiclesByOrganizationAsync(int organizationId);
    }
}
