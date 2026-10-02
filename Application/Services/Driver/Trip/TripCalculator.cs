using fleetops_backend.Models;
using NetTopologySuite.Geometries;

namespace fleetops_backend.Application.Services
{
    public class TripCalculator : ITripCalculator
    {
        public Point? CreatePoint(double? latitude, double? longitude)
        {
            if (!latitude.HasValue || !longitude.HasValue)
            {
                return null;
            }

            return new Point(longitude.Value, latitude.Value)
            {
                SRID = 4326
            };
        }

        public (DateTimeOffset StartUtc, DateTimeOffset EndUtc)
            GetInspectionDateRange(DateTimeOffset now)
        {
            var offset = TimeSpan.FromHours(7);
            var localDate = now.ToOffset(offset).Date;
            var startUtc = new DateTimeOffset(localDate, offset)
                .ToUniversalTime();

            return (startUtc, startUtc.AddDays(1));
        }

        public void CalculateMetrics(
            TripDetail tripDetail,
            int? odometerAwal,
            int? odometerAkhir,
            DateTimeOffset waktuMulai,
            DateTimeOffset waktuSelesai)
        {
            if (odometerAwal.HasValue && odometerAkhir.HasValue)
            {
                tripDetail.Jarak =
                    odometerAkhir.Value - odometerAwal.Value;
            }

            var duration = waktuSelesai - waktuMulai;
            tripDetail.Waktu = (int)duration.TotalSeconds;

            if (tripDetail.Jarak > 0 && duration.TotalSeconds > 0)
            {
                tripDetail.AvgSpeed =
                    (int)(tripDetail.Jarak /
                    (duration.TotalSeconds / 3600));
            }
        }
    }
}