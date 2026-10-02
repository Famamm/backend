using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IAdminVehicleMapper
    {
        Vehicle ToVehicle(VehicleDTO dto, int organizationId, string imageUrl);
        GetVehicleDTO ToVehicleDto(Vehicle vehicle, VehicleEfficiencyDTO? efficiency);
        GetVehicleDTO ToSummaryDto(Vehicle vehicle);
        void ApplyUpdate(Vehicle vehicle, UpdateVehicleDTO dto);
    }
}
