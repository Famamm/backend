using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class DriverProfileMapper : IDriverProfileMapper
    {
        public GetDriverProfileDTO MapProfile(
            Driver driver,
            DriverStatisticsDto statistics,
            DriverVehicleDto? activeVehicle,
            List<DriverVehicleDto> availableVehicles)
        {
            return new GetDriverProfileDTO
            {
                DriverId = driver.Id,
                NamaDriver = driver.User?.FullName ?? string.Empty,
                Email = driver.User?.Email ?? string.Empty,
                Address = driver.User?.Address,
                PhotoUrl = driver.User?.ProfilePhoto,
                Statistics = statistics,
                OrganizationId = driver.OrganizationId,
                OrganizationName = driver.Organization?.OrganizationName,
                KendaraanAktif = activeVehicle,
                KendaraanTersedia = availableVehicles,
                Dokumen = MapDocument(driver)
            };
        }

        public DriverVehicleDto? MapVehicle(Vehicle? vehicle)
        {
            if (vehicle == null)
            {
                return null;
            }

            return new DriverVehicleDto
            {
                VehicleId = vehicle.Id,
                VehicleName = vehicle.VehicleName,
                PlatNomor = vehicle.PlatNomor,
                Tipe = vehicle.Tipe,
                TipeBensin = vehicle.TipeBensin,
                Status = vehicle.Status,
                ImageUrl = vehicle.ImageUrl,
                Odometer = vehicle.Odometer
            };
        }

        public DriverDocumentDto? MapDocument(Driver driver)
        {
            if (string.IsNullOrEmpty(driver.JenisLisensi) || !driver.TanggalBerlaku.HasValue)
            {
                return null;
            }

            return new DriverDocumentDto
            {
                JenisLisensi = driver.JenisLisensi,
                FotoLisensi = driver.FotoLisensi,
                TanggalBerlaku = driver.TanggalBerlaku.Value,
                Status = driver.Status ?? "Active"
            };
        }
    }
}
