using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IDriverInspectionService
    {
        Task<GetInspectionDTO?> CreateInspectionAsync(CreateInspectionDTO dto);
        Task<List<GetInspectionDTO>> GetInspectionsByDriverAsync(int driverId);
        Task<List<GetInspectionDTO>> GetInspectionsByUserIdAsync(int userId);
        Task<List<GetInspectionDTO>> GetTodayInspectionsByUserIdAsync(int userId);
        Task<List<GetInspectionDTO>> GetInspectionsByVehicleAsync(int vehicleId);
        Task<GetInspectionDTO?> UpdateInspectionAsync(int id, UpdateInspectionDTO dto);
        Task<bool> DeleteInspectionAsync(int id);
        Task<GetDetailInspectionDTO?> GetInspectionDetailByIdAsync(int id);
        Task<List<InspectionItemDTO>> GetInspectionItemsAsync();
    }
}
