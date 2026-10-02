using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IDriverDamageReportService
    {
        Task<DamageReportDTO> CreateAsync(int userId, int tripId, CreateDamageReportDTO dto);
        Task<List<DamageReportDTO>> GetByTripAsync(int userId, int tripId);
    }
}
