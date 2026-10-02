using Microsoft.EntityFrameworkCore;
using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;

namespace fleetops_backend.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private const string DeletedStatus = "Deleted";
        private const string FinishedTripStatus = "Finished";

        private static readonly TimeSpan WibOffset = DashboardRangeWindow.WibOffset;

        private const string FleetManagerRole = "FleetManager";


        private readonly AppDbContext _context;

        public AdminDashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardDTO> GetAdminDashboardAsync(int adminId, int? bulan = null, int? tahun = null, string? rangeArmada = null, string? rangeBbm = null)
        {
            var admin = await _context.Users
                .Include(u => u.Organization)
                .FirstOrDefaultAsync(u => u.Id == adminId);

            if (admin == null)
            {
                throw new KeyNotFoundException($"Admin dengan ID {adminId} tidak ditemukan");
            }

            if (admin.Organization == null)
            {
                throw new KeyNotFoundException("Organisasi admin tidak ditemukan");
            }

            var organizationId = admin.OrganizationId;

            var nowWib = DateTimeOffset.UtcNow.ToOffset(WibOffset);
            var periodMonth = bulan ?? nowWib.Month;
            var periodYear = tahun ?? nowWib.Year;

            if (periodMonth < 1 || periodMonth > 12)
            {
                throw new ArgumentOutOfRangeException(nameof(bulan), "Bulan harus antara 1 dan 12");
            }

            var todayWib = DateOnly.FromDateTime(nowWib.DateTime);

            // Setiap chart punya rentang waktunya sendiri.
            var armadaWindow = DashboardRangeWindow.Create(rangeArmada, todayWib);
            var bbmWindow = DashboardRangeWindow.Create(rangeBbm, todayWib);

            // Npgsql hanya menerima offset 0 (UTC) untuk kolom 'timestamp with time zone',
            // jadi parameter query harus dikonversi ke UTC terlebih dahulu.
            var armadaStartUtc = armadaWindow.StartWib.ToUniversalTime();
            var armadaEndUtc = armadaWindow.EndWibExclusive.ToUniversalTime();
            var bbmStartUtc = bbmWindow.StartWib.ToUniversalTime();
            var bbmEndUtc = bbmWindow.EndWibExclusive.ToUniversalTime();

            var totalDrivers = await _context.Drivers
                .CountAsync(d => d.OrganizationId == organizationId
                    && (d.Status == null || d.Status != DeletedStatus));

            var totalVehicles = await _context.Vehicles
                .CountAsync(v => v.OrganizationId == organizationId);

            var totalTrips = await _context.Trips
                .CountAsync(t => t.OrganizationId == organizationId
                    && t.Status == FinishedTripStatus);

            var tripsInPeriod = _context.Trips
                .Where(t => t.OrganizationId == organizationId
                    && t.Status == FinishedTripStatus
                    && t.WaktuSelesai != null
                    && t.WaktuSelesai >= armadaStartUtc
                    && t.WaktuSelesai < armadaEndUtc);

            var totalPerjalanan = await tripsInPeriod.CountAsync();

            var totalJarak = await tripsInPeriod
                .SelectMany(t => t.Detail)
                .SumAsync(d => (long?)d.Jarak) ?? 0;

            var totalDurasi = await tripsInPeriod
                .SelectMany(t => t.Detail)
                .SumAsync(d => (long?)d.Waktu) ?? 0;

            var totalJam = totalDurasi / 3600.0;

            var rincianAktivitasArmada = new AdminRincianAktivitasArmadaDTO
            {
                TotalJarak = totalJarak,
                TotalDurasi = totalDurasi,
                RataRataKecepatan = totalJam > 0
                    ? Math.Round(totalJarak / totalJam, 2)
                    : 0
            };

            var activityTripDates = await _context.Trips
                .Where(t => t.OrganizationId == organizationId
                    && t.Status == FinishedTripStatus
                    && t.WaktuSelesai != null
                    && t.WaktuSelesai >= armadaStartUtc
                    && t.WaktuSelesai < armadaEndUtc)
                .Select(t => t.WaktuSelesai!.Value)
                .ToListAsync();

            var activityDatesWib = activityTripDates
                .Select(dt => DateOnly.FromDateTime(dt.ToOffset(WibOffset).DateTime));

            var aktivitasArmada = armadaWindow.Granularity == DashboardRangeGranularity.Monthly
                ? FleetActivityChartBuilder.BuildMonthly(armadaWindow.StartDate, armadaWindow.BucketCount, activityDatesWib)
                : FleetActivityChartBuilder.BuildDaily(armadaWindow.StartDate, armadaWindow.BucketCount, activityDatesWib);

            var fuelInWindow = await _context.Fuels
                .Where(f => f.OrganizationId == organizationId
                    && f.CreatedAt >= bbmStartUtc.UtcDateTime
                    && f.CreatedAt < bbmEndUtc.UtcDateTime)
                .Select(f => new
                {
                    f.CreatedAt,
                    f.JmlLiter,
                    f.HargaTotal
                })
                .ToListAsync();

            var fuelByDate = fuelInWindow
                .GroupBy(f => bbmWindow.ToBucket(DateOnly.FromDateTime(new DateTimeOffset(DateTime.SpecifyKind(f.CreatedAt, DateTimeKind.Utc)).ToOffset(WibOffset).DateTime)))
                .ToDictionary(
                    g => g.Key,
                    g => new
                    {
                        TotalLiter = g.Sum(x => x.JmlLiter),
                        TotalHarga = g.Sum(x => (long)x.HargaTotal)
                    });

            var aktivitasBbm = new List<AdminBbmActivityDayDTO>(bbmWindow.BucketCount);

            for (var index = 0; index < bbmWindow.BucketCount; index++)
            {
                var date = bbmWindow.BucketAt(index);
                fuelByDate.TryGetValue(date, out var stats);

                aktivitasBbm.Add(new AdminBbmActivityDayDTO
                {
                    Tanggal = date,
                    TotalLiter = stats?.TotalLiter ?? 0m,
                    TotalHarga = stats?.TotalHarga ?? 0L
                });
            }

            var fuelInPeriod = _context.Fuels
                .Where(f => f.OrganizationId == organizationId
                    && f.CreatedAt >= bbmStartUtc.UtcDateTime
                    && f.CreatedAt < bbmEndUtc.UtcDateTime);

            var totalBbmLiter = await fuelInPeriod.SumAsync(f => (decimal?)f.JmlLiter) ?? 0;
            var totalHargaBbm = await fuelInPeriod.SumAsync(f => (long?)f.HargaTotal) ?? 0;

            var fuelWithPrice = await fuelInPeriod
                .Where(f => f.FuelPrice != null)
                .Select(f => new
                {
                    FuelType = f.FuelPrice!.FuelType,
                    f.JmlLiter,
                    f.HargaTotal
                })
                .ToListAsync();

            var rincianJenisBbm = fuelWithPrice
                .GroupBy(f => f.FuelType)
                .Select(g => new AdminJenisBbmDTO
                {
                    Jenis = g.Key,
                    TotalLiter = g.Sum(f => f.JmlLiter),
                    TotalHarga = g.Sum(f => (long)f.HargaTotal)
                })
                .ToList();

            var jenisBbm = rincianJenisBbm.Select(r => r.Jenis).ToList();

            var totalFleetManagers = await _context.Users
                .CountAsync(u => u.OrganizationId == organizationId
                    && u.Role!.Name == FleetManagerRole);

            return new AdminDashboardDTO
            {
                OrganizationName = admin.Organization.OrganizationName,
                TotalDrivers = totalDrivers,
                TotalVehicles = totalVehicles,
                TotalTrips = totalTrips,
                Bulan = periodMonth,
                Tahun = periodYear,
                RangeArmada = armadaWindow.Range,
                RangeBbm = bbmWindow.Range,
                AktivitasArmada = aktivitasArmada,
                TotalPerjalanan = totalPerjalanan,
                RincianAktivitasArmada = rincianAktivitasArmada,
                AktivitasBbm = aktivitasBbm,
                TotalBbmLiter = totalBbmLiter,
                TotalHargaBbm = totalHargaBbm,
                JenisBbm = jenisBbm,
                RincianJenisBbm = rincianJenisBbm,
                TotalFleetManagers = totalFleetManagers
            };
        }
    }
}