using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class AssignmentMapper : IAssignmentMapper
    {
        public CurrentDriverDTO ToCurrentDriverDto(Vehicle vehicle)
        {
            return new CurrentDriverDTO
            {
                VehicleId = vehicle.Id,
                VehicleName = vehicle.VehicleName,
                PlatNomor = vehicle.PlatNomor,
                Tipe = vehicle.Tipe,
                Odometer = vehicle.Odometer,
                VehicleStatus = vehicle.Status,
                DriverId = vehicle.Driver?.Id,
                NamaDriver = vehicle.Driver?.User?.FullName,
                Username = vehicle.Driver?.User?.Username,
                ProfilePhoto = vehicle.Driver?.User?.ProfilePhoto,
                JenisLisensi = vehicle.Driver?.JenisLisensi,
                DriverStatus = vehicle.Driver?.Status
            };
        }

        public AvailableDriverDTO ToAvailableDriverDto(Driver driver)
        {
            return new AvailableDriverDTO
            {
                DriverId = driver.Id,
                UserId = driver.UserId,
                NamaDriver = driver.User?.FullName ?? string.Empty,
                Username = driver.User?.Username ?? string.Empty,
                ProfilePhoto = driver.User?.ProfilePhoto,
                JenisLisensi = driver.JenisLisensi,
                TanggalBerlaku = driver.TanggalBerlaku,
                Status = driver.Status
            };
        }
    }
}
