using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IFleetManagerAssignmentService
    {
        Task<CurrentDriverDTO> AssignDriverAsync(int organizationId, AssignVehicleDTO dto);
        Task<CurrentDriverDTO> UnassignDriverAsync(int organizationId, int vehicleId);
        Task<CurrentDriverDTO> GetCurrentDriverAsync(int organizationId, int vehicleId);
        Task<List<AvailableDriverDTO>> GetAvailableDriversAsync(int organizationId);
    }
}
