using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class FuelMapper : IFuelMapper
    {
        public TripFuelRecordDTO ToFuelRecordDto(Fuel fuel)
        {
            return new TripFuelRecordDTO
            {
                FuelId = fuel.Id,
                JmlLiter = (decimal)fuel.JmlLiter,
                HargaTotal = fuel.HargaTotal,
                FuelType = fuel.FuelPrice?.FuelType ?? "Unknown",
                WaktuPengisian = fuel.CreatedAt,
                VehicleId = fuel.VehicleId,
                VehicleName = fuel.Vehicle?.VehicleName ?? "Unknown",
                PlatNomor = fuel.Vehicle?.PlatNomor ?? "Unknown"
            };
        }

        public TripFuelRecordDetailDTO ToFuelRecordDetailDto(Fuel fuel)
        {
            return new TripFuelRecordDetailDTO
            {
                FuelId = fuel.Id,
                JmlLiter = (decimal)fuel.JmlLiter,
                HargaTotal = fuel.HargaTotal,
                FuelType = fuel.FuelPrice?.FuelType ?? "Unknown",
                ImageUrlPath = fuel.ImageUrlPath,
                WaktuPengisian = fuel.CreatedAt,
                VehicleId = fuel.VehicleId,
                VehicleName = fuel.Vehicle?.VehicleName ?? "Unknown",
                PlatNomor = fuel.Vehicle?.PlatNomor ?? "Unknown",
                OdometerIsi = fuel.OdometerIsi,
                Catatan = fuel.Catatan ?? string.Empty,
                Latitude = fuel.LokasiPengisian?.Y,
                Longitude = fuel.LokasiPengisian?.X
            };
        }
    }
}
