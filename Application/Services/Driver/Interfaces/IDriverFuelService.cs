using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IDriverFuelService
    {
        Task<(bool success, Fuel? fuel, string? errorMessage)> LogFuelAsync(int userId, int organizationId, CreateFuelDTO dto);
        Task<VehicleEfficiencyDTO?> GetVehicleEfficiencyByVehicleIdAsync(int vehicleId);
        Task<List<VehicleEfficiencyDTO>> GetVehicleEfficiencyListAsync();
        Task<List<TripFuelRecordDTO>> GetFuelHistoryByDriverAsync(int driverId);
        Task<List<TripFuelRecordDetailDTO>> GetFuelDetailByIdAsync(int fuelId);
        Task<List<TripFuelRecordDTO>> GetFuelHistoryByUserIdAsync(int userId);
    }
}
