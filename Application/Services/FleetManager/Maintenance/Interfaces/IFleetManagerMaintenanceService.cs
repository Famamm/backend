using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IFleetManagerMaintenanceService
    {
        Task<MaintenanceDTO> CreateAsync(int organizationId, int vehicleId, MaintenanceScheduleDTO dto);
        Task<List<MaintenanceDTO>> GetByVehicleAsync(int organizationId, int vehicleId);
        Task<MaintenanceDTO> GetByIdAsync(int organizationId, int maintenanceId);
        Task<MaintenanceDTO> UpdateAsync(int organizationId, int maintenanceId, MaintenanceScheduleDTO dto);
        Task DeleteAsync(int organizationId, int maintenanceId);
        Task<MaintenanceDTO> UpdateStatusAsync(int organizationId, int maintenanceId, UpdateMaintenanceStatusDTO dto);
        Task<List<VehicleMaintenanceStatusDTO>> GetFleetStatusAsync(int organizationId);
    }
}
