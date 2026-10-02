using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IAdminDriverService
    {
        Task<(Driver driver, string? generatedPassword, string? username)> CreateDriverAsync(int organizationId, CreateDriverDTO dto);
        Task<List<Driver>> GetDriversAsync(int organizationId);
        Task<DriverDetailDTO> GetDriverByIdAsync(int organizationId, int driverId);
        Task<Driver> UpdateDriverAsync(int organizationId, int driverId, UpdateDriverDTO dto);
        Task DeleteDriverAsync(int organizationId, int driverId);
        Task SetDriverPasswordAsync(int organizationId, int driverId, SetDriverPasswordDTO dto);
        Task<(Driver driver, string generatedPassword)> ResetDriverPasswordAsync(int organizationId, int driverId);
    }
}
