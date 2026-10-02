using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDTO> GetAdminDashboardAsync(int adminId, int? bulan = null, int? tahun = null, string? rangeArmada = null, string? rangeBbm = null);
    }
}
