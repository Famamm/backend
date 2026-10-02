using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class AdminDriverMapper : IAdminDriverMapper
    {
        public Driver ToDriver(
            CreateDriverDTO dto,
            int userId,
            int organizationId,
            string? licensePhotoUrl)
        {
            return new Driver
            {
                UserId = userId,
                OrganizationId = organizationId,
                JenisLisensi = dto.JenisLisensi,
                FotoLisensi = licensePhotoUrl,
                TanggalBerlaku = dto.TanggalBerlaku?.ToUniversalTime(),
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Active" : dto.Status
            };
        }

        public DriverDetailDTO ToDriverDetailDto(Driver driver, int totalTrip)
        {
            return new DriverDetailDTO
            {
                DriverId = driver.Id,
                UserId = driver.UserId,
                FullName = driver.User?.FullName,
                Username = driver.User?.Username,
                Email = driver.User?.Email,
                Address = driver.User?.Address,
                ProfilePhoto = driver.User?.ProfilePhoto,
                JenisLisensi = driver.JenisLisensi,
                FotoLisensi = driver.FotoLisensi,
                TanggalBerlaku = driver.TanggalBerlaku,
                Status = driver.Status,
                TotalTrip = totalTrip
            };
        }
    }
}
