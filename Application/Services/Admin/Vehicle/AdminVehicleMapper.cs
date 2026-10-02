using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class AdminVehicleMapper : IAdminVehicleMapper
    {
        public Vehicle ToVehicle(VehicleDTO dto, int organizationId, string imageUrl)
        {
            return new Vehicle
            {
                OrganizationId = organizationId,
                VehicleName = dto.VehicleName,
                PlatNomor = dto.PlatNomor,
                TahunPembuatan = dto.TahunPembuatan,
                Odometer = dto.Odometer,
                Tipe = dto.Tipe,
                Status = dto.Status,
                MuatanMax = dto.MuatanMax,
                TipeBensin = dto.TipeBensin,
                ImageUrl = imageUrl
            };
        }

        public GetVehicleDTO ToVehicleDto(Vehicle vehicle, VehicleEfficiencyDTO? efficiency)
        {
            return new GetVehicleDTO
            {
                VehicleId = vehicle.Id,
                VehicleName = vehicle.VehicleName,
                PlatNomor = vehicle.PlatNomor,
                Tipe = vehicle.Tipe,
                TipeBensin = vehicle.TipeBensin.ToString(),
                Odometer = vehicle.Odometer,
                MuatanMax = vehicle.MuatanMax,
                Status = vehicle.Status,
                TahunPembuatan = vehicle.TahunPembuatan,
                ImageUrl = vehicle.ImageUrl ?? string.Empty,
                TotalJarak = efficiency?.TotalJarak,
                TotalBensin = efficiency?.TotalBensin,
                SkorEfisiensi = efficiency?.SkorEfisiensi,
                VsBulanLalu = efficiency?.VsBulanLalu,
                StatusEfisiensi = efficiency?.StatusEfisiensi
            };
        }

        public GetVehicleDTO ToSummaryDto(Vehicle vehicle)
        {
            return new GetVehicleDTO
            {
                VehicleId = vehicle.Id,
                VehicleName = vehicle.VehicleName,
                PlatNomor = vehicle.PlatNomor,
                Status = vehicle.Status
            };
        }

        public void ApplyUpdate(Vehicle vehicle, UpdateVehicleDTO dto)
        {
            vehicle.VehicleName = dto.VehicleName ?? vehicle.VehicleName;
            vehicle.PlatNomor = dto.PlatNomor ?? vehicle.PlatNomor;
            vehicle.TahunPembuatan = dto.TahunPembuatan ?? vehicle.TahunPembuatan;
            vehicle.Odometer = dto.Odometer ?? vehicle.Odometer;
            vehicle.Tipe = dto.Tipe ?? vehicle.Tipe;
            vehicle.Status = dto.Status ?? vehicle.Status;
            vehicle.MuatanMax = dto.MuatanMax ?? vehicle.MuatanMax;
            vehicle.TipeBensin = dto.TipeBensin ?? vehicle.TipeBensin;
        }
    }
}
