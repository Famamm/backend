using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IDriverProfileMapper
    {
        GetDriverProfileDTO MapProfile(
            Driver driver,
            DriverStatisticsDto statistics,
            DriverVehicleDto? activeVehicle,
            List<DriverVehicleDto> availableVehicles);

        DriverVehicleDto? MapVehicle(Vehicle? vehicle);
        DriverDocumentDto? MapDocument(Driver driver);
    }
}
