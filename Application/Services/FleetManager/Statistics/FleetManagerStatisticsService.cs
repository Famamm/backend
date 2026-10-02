using Microsoft.EntityFrameworkCore;
using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;

namespace fleetops_backend.Application.Services
{
    public class FleetManagerStatisticsService : IFleetManagerStatisticsService
    {
        private const string ActiveVehicleStatus = "Active";
        private const string MaintenanceVehicleStatus = "Maintenance";
        private const string FinishedTripStatus = "Finished";
        private const int ActivityWindowDays = 7;

        private static readonly TimeSpan WibOffset = TimeSpan.FromHours(7);

        private readonly AppDbContext _context;

        public FleetManagerStatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FleetStatisticsDTO> GetStatisticsAsync(int organizationId, int? bulan, int? tahun)
        {
            var nowWib = DateTimeOffset.UtcNow.ToOffset(WibOffset);
            var periodMonth = bulan ?? nowWib.Month;
            var periodYear = tahun ?? nowWib.Year;

            if (periodMonth < 1 || periodMonth > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(bulan), "Bulan harus antara 1 dan 12");
            }

            var periodStart = new DateTimeOffset(periodYear, periodMonth, 1, 0, 0, 0, WibOffset);
            var periodEnd = periodStart.AddMonths(1);

            // Npgsql hanya menerima offset 0 (UTC) untuk kolom 'timestamp with time zone',
            // jadi parameter query harus dikonversi ke UTC terlebih dahulu.
            var periodStartUtc = periodStart.ToUniversalTime();
            var periodEndUtc = periodEnd.ToUniversalTime();

            var vehicleStatuses = await _context.Vehicles
                .Where(v => v.OrganizationId == organizationId)
                .Select(v => v.Status)
                .ToListAsync();

            var totalKendaraan = vehicleStatuses.Count;
            var kendaraanAktif = vehicleStatuses.Count(s => s == ActiveVehicleStatus);
            var kendaraanDalamServis = vehicleStatuses.Count(s => s == MaintenanceVehicleStatus);
            var kendaraanTidakAktif = totalKendaraan - kendaraanAktif - kendaraanDalamServis;

            var tripsInPeriod = _context.Trips
                .Where(t => t.OrganizationId == organizationId
                    && t.Status == FinishedTripStatus
                    && t.WaktuSelesai != null
                    && t.WaktuSelesai >= periodStartUtc
                    && t.WaktuSelesai < periodEndUtc);

            var totalPerjalanan = await tripsInPeriod.CountAsync();
            var totalJarak = await tripsInPeriod
                .SelectMany(t => t.Detail)
                .SumAsync(d => (long?)d.Jarak) ?? 0;

            var todayWib = DateOnly.FromDateTime(nowWib.DateTime);
            var windowStartWib = todayWib.AddDays(-(ActivityWindowDays - 1));
            var windowStart = new DateTimeOffset(windowStartWib.ToDateTime(TimeOnly.MinValue), WibOffset);
            var windowEnd = new DateTimeOffset(todayWib.AddDays(1).ToDateTime(TimeOnly.MinValue), WibOffset);

            var windowStartUtc = windowStart.ToUniversalTime();
            var windowEndUtc = windowEnd.ToUniversalTime();

            var activityTripDates = await _context.Trips
                .Where(t => t.OrganizationId == organizationId
                    && t.Status == FinishedTripStatus
                    && t.WaktuSelesai != null
                    && t.WaktuSelesai >= windowStartUtc
                    && t.WaktuSelesai < windowEndUtc)
                .Select(t => t.WaktuSelesai!.Value)
                .ToListAsync();

            var aktivitasArmada = FleetActivityChartBuilder.Build(
                todayWib,
                ActivityWindowDays,
                activityTripDates.Select(dt => DateOnly.FromDateTime(dt.ToOffset(WibOffset).DateTime)));

            var fuelInPeriod = _context.Fuels
                .Where(f => f.OrganizationId == organizationId
                    && f.CreatedAt >= periodStart.UtcDateTime
                    && f.CreatedAt < periodEnd.UtcDateTime);

            var totalBbmLiter = await fuelInPeriod.SumAsync(f => (decimal?)f.JmlLiter) ?? 0;
            var totalHargaBbm = await fuelInPeriod.SumAsync(f => (long?)f.HargaTotal) ?? 0;
            var jenisBbm = await fuelInPeriod
                .Where(f => f.FuelPrice != null)
                .Select(f => f.FuelPrice!.FuelType)
                .Distinct()
                .ToListAsync();

            return new FleetStatisticsDTO
            {
                Bulan = periodMonth,
                Tahun = periodYear,
                TotalKendaraan = totalKendaraan,
                KendaraanAktif = kendaraanAktif,
                KendaraanDalamServis = kendaraanDalamServis,
                KendaraanTidakAktif = kendaraanTidakAktif,
                AktivitasArmada = aktivitasArmada,
                TotalPerjalanan = totalPerjalanan,
                TotalJarak = totalJarak,
                TotalBbmLiter = totalBbmLiter,
                TotalHargaBbm = totalHargaBbm,
                JenisBbm = jenisBbm
            };
        }
    }
}
