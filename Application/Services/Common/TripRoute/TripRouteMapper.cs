using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public static class TripRouteMapper
    {
        private static readonly TimeSpan WibOffset = TimeSpan.FromHours(7);

        public static TripGpsPointDTO ToPointDto(TripGpsPoint point)
        {
            return new TripGpsPointDTO
            {
                ClientId = point.ClientId,
                // Coordinate disimpan sebagai geography Point(longitude, latitude),
                // jadi X adalah longitude dan Y adalah latitude — bukan sebaliknya.
                Latitude = point.Coordinate.Y,
                Longitude = point.Coordinate.X,
                CapturedAt = point.CapturedAt.ToOffset(WibOffset),
                AccuracyMeters = point.AccuracyMeters
            };
        }

        public static TripRouteDTO ToRouteDto(Trip trip, List<TripGpsPoint> points)
        {
            return new TripRouteDTO
            {
                TripId = trip.Id,
                DriverId = trip.DriverId,
                DriverName = trip.Driver?.User?.FullName,
                VehicleId = trip.VehicleId,
                VehicleName = trip.Vehicle?.VehicleName,
                PlatNomor = trip.Vehicle?.PlatNomor,
                Status = trip.Status,
                LokasiAwal = trip.LokasiAwal,
                LokasiAkhir = trip.LokasiAkhir,
                WaktuMulai = trip.WaktuMulai.ToOffset(WibOffset),
                WaktuSelesai = trip.WaktuSelesai?.ToOffset(WibOffset),
                PointCount = points.Count,
                Points = points.Select(ToPointDto).ToList()
            };
        }

        public static DriverLiveLocationDTO ToLiveLocationDto(Trip trip, TripGpsPoint? lastPoint)
        {
            return new DriverLiveLocationDTO
            {
                TripId = trip.Id,
                DriverId = trip.DriverId,
                DriverName = trip.Driver?.User?.FullName,
                VehicleId = trip.VehicleId,
                VehicleName = trip.Vehicle?.VehicleName,
                PlatNomor = trip.Vehicle?.PlatNomor,
                LokasiAwal = trip.LokasiAwal,
                WaktuMulai = trip.WaktuMulai.ToOffset(WibOffset),
                LastPoint = lastPoint == null ? null : ToPointDto(lastPoint)
            };
        }
    }
}
