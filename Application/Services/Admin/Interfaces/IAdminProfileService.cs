using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.DTOs.Admin;

namespace fleetops_backend.Application.Services
{
    public interface IAdminProfileService
    {
        Task<AccountDTO> GetAdminProfileAsync(int adminId);
        Task<UpdateAdminProfileDTO> UpdateAdminProfileAsync(int adminId, UpdateAdminProfileDTO dto);
    }
}
