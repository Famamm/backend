using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IDamageReportService
    {
        Task<List<DamageReportDTO>> GetAllAsync(int organizationId, string? status);
        Task<DamageReportDTO> GetByIdAsync(int organizationId, int id);
        Task<DamageReportDTO> UpdateStatusAsync(int organizationId, int id, UpdateDamageReportStatusDTO dto);
    }
}
