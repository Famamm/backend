using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IAdminDriverMapper
    {
        Driver ToDriver(CreateDriverDTO dto, int userId, int organizationId, string? licensePhotoUrl);
        DriverDetailDTO ToDriverDetailDto(Driver driver, int totalTrip);
    }
}
