using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class TripMapper : ITripMapper
    {
        public GetTripDTO ToTripDto(Trip trip, TripDetail? detail)
        {
             return new GetTripDTO
            {
                TripId = trip.Id,
                LokasiAwal = trip.LokasiAwal ?? string.Empty,
                LokasiAkhir = trip.LokasiAkhir ?? string.Empty,
                WaktuMulai = trip.WaktuMulai,
                WaktuSelesai = trip.WaktuSelesai ?? DateTimeOffset.MinValue,
                JarakTempuh = detail?.Jarak ?? 0,
                Durasi = detail?.Waktu ?? 0,
                Status = trip.Status
            };
        }

        public GetDetailTripDTO ToDetailTripDto(
            Trip trip,
            Driver driver,
            Vehicle vehicle)
        {
             return new GetDetailTripDTO
            {
                TripId = trip.Id,
                DriverId = driver.Id,
                VehicleId = vehicle.Id,
                LokasiAwal = trip.LokasiAwal,
                LokasiAkhir = trip.LokasiAkhir,
                WaktuMulai = trip.WaktuMulai,
                WaktuSelesai = trip.WaktuSelesai,
                Catatan = trip.Catatan,
                Details = trip.Detail.Select(detail => new TripDetailDTO
                {
                    Jarak = detail.Jarak,
                    AvgSpeed = detail.AvgSpeed,
                    Waktu = detail.Waktu,
                    ImageUrl = detail.ImageUrl
                }).ToList()
            };
        }

        public GetActiveTripDTO ToActiveTripDto(
            Trip trip,
            DateTimeOffset currentTime)
        {
            return new GetActiveTripDTO
            {
                TripId = trip.Id,
                DriverId = trip.DriverId,
                VehicleId = trip.VehicleId,
                VehicleName = trip.Vehicle?.VehicleName ?? string.Empty,
                PlatNomor = trip.Vehicle?.PlatNomor ?? string.Empty,
                LokasiAwal = trip.LokasiAwal ?? string.Empty,
                LokasiAkhir = trip.LokasiAkhir,
                WaktuMulai = trip.WaktuMulai,
                OdometerAwal = trip.OdometerAwal,
                Status = trip.Status,
                DurasiDetik = (int)(
                    currentTime - trip.WaktuMulai).TotalSeconds
            };
        }
    }
}