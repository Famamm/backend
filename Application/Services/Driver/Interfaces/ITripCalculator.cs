using fleetops_backend.Models;
using NetTopologySuite.Geometries;

namespace fleetops_backend.Application.Services
{
    public interface ITripCalculator
    {
        Point? CreatePoint(double? latitude, double? longitude);
        (DateTimeOffset StartUtc, DateTimeOffset EndUtc) GetInspectionDateRange(DateTimeOffset now);
        void CalculateMetrics(
            TripDetail tripDetail,
            int? odometerAwal,
            int? odometerAkhir,
            DateTimeOffset waktuMulai,
            DateTimeOffset waktuSelesai);
    }
}
