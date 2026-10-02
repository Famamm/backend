using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace fleetops_backend.Application.Services
{
    public class DriverFuelService : IDriverFuelService
    {
        private readonly AppDbContext _context;
        private readonly IFuelValidator _validator;
        private readonly IFuelMapper _mapper;
        private readonly IFuelPhotoService _photoService;
        private readonly IFuelEfficiencyCalculator _efficiencyCalculator;
        private readonly IFuelAnomalyService _anomalyService;

        public DriverFuelService(
            AppDbContext context,
            IFuelValidator validator,
            IFuelMapper mapper,
            IFuelPhotoService photoService,
            IFuelEfficiencyCalculator efficiencyCalculator,
            IFuelAnomalyService anomalyService)
        {
            _context = context;
            _validator = validator;
            _mapper = mapper;
            _photoService = photoService;
            _efficiencyCalculator = efficiencyCalculator;
            _anomalyService = anomalyService;
        }

        public async Task<(bool success, Fuel? fuel, string? errorMessage)> LogFuelAsync(int userId, int organizationId, CreateFuelDTO dto)
        {
            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d => d.UserId == userId && d.OrganizationId == organizationId);

            _validator.ValidateDriverExists(driver);

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == dto.VehicleId && v.OrganizationId == organizationId);

            _validator.ValidateVehicleExists(vehicle);
            _validator.ValidateVehicleAssignedToDriver(vehicle!, driver!.Id);

            var fuelPrice = await _context.FuelPrices
                .FirstOrDefaultAsync(f => f.FuelType.ToLower() == dto.FuelType.ToLower());

            _validator.ValidateFuelPriceExists(fuelPrice, dto.FuelType);

            _validator.ValidateOdometer(dto.OdometerIsi, vehicle!.Odometer);
            _validator.ValidateLocation(dto.Latitude, dto.Longitude);

            var (uploadSuccess, imageUrl, uploadError) = await _photoService.UploadFuelPhotoAsync(dto.FuelPhoto);
            if (!uploadSuccess)
            {
                return (false, null, uploadError);
            }

            var fuel = new Fuel
            {
                VehicleId = dto.VehicleId,
                FuelPriceId = fuelPrice!.Id,
                JmlLiter = dto.JmlLiter,
                OrganizationId = organizationId,
                DriverId = driver.Id,
                HargaTotal = (int)(fuelPrice.PricePerLiter * dto.JmlLiter),
                OdometerIsi = dto.OdometerIsi,
                Catatan = dto.Catatan,
                ImageUrlPath = imageUrl,
                LokasiPengisian = CreatePoint(dto.Latitude, dto.Longitude),
                CreatedAt = DateTime.UtcNow
            };

            _context.Fuels.Add(fuel);
            await _context.SaveChangesAsync();

            await _anomalyService.CheckAsync(vehicle, fuel);

            return (true, fuel, null);
        }

        public async Task<VehicleEfficiencyDTO?> GetVehicleEfficiencyByVehicleIdAsync(int vehicleId)
        {
            var now = DateTime.UtcNow;
            var (startOfCurrentMonth, startOfLastMonth, endOfLastMonth) = _efficiencyCalculator.GetMonthRanges(now);
            var startOfLastMonthOffset = new DateTimeOffset(startOfLastMonth);

            var trips = await _context.Trips
                .Where(t => t.VehicleId == vehicleId
                        && t.OdometerAwal != null
                        && t.OdometerAkhir != null
                        && t.WaktuMulai >= startOfLastMonthOffset)
                .ToListAsync();

            var fuels = await _context.Fuels
                .Where(f => f.VehicleId == vehicleId
                        && f.CreatedAt >= startOfLastMonth)
                .ToListAsync();

            var result = _efficiencyCalculator.Calculate(trips, fuels, startOfCurrentMonth, startOfLastMonth, endOfLastMonth);

            return new VehicleEfficiencyDTO
            {
                TotalJarak = result.TotalJarakCurrent,
                TotalBensin = (int)result.TotalBensinCurrent,
                SkorEfisiensi = Math.Round(result.SkorCurrent, 1),
                VsBulanLalu = Math.Round(result.VsBulanLalu, 1),
                StatusEfisiensi = ""
            };
        }

        public async Task<List<VehicleEfficiencyDTO>> GetVehicleEfficiencyListAsync()
        {
            var now = DateTime.UtcNow;
            var (startOfCurrentMonth, startOfLastMonth, endOfLastMonth) = _efficiencyCalculator.GetMonthRanges(now);
            var startOfLastMonthOffset = new DateTimeOffset(startOfLastMonth);

            var vehicles = await _context.Vehicles.ToListAsync();

            // Perbaikan: Gunakan WaktuMulai
            var trips = await _context.Trips
                .Where(t => t.WaktuMulai >= startOfLastMonthOffset && t.OdometerAwal != null && t.OdometerAkhir != null)
                .ToListAsync();

            var fuels = await _context.Fuels
                .Where(f => f.CreatedAt >= startOfLastMonth)
                .ToListAsync();

            var rawCalculations = new List<dynamic>();
            var currentMonthScores = new List<Tuple<string, double>>();

            foreach (var vehicle in vehicles)
            {
                var vehicleTrips = trips.Where(t => t.VehicleId == vehicle.Id);
                var vehicleFuels = fuels.Where(f => f.VehicleId == vehicle.Id);

                var calc = _efficiencyCalculator.Calculate(vehicleTrips, vehicleFuels, startOfCurrentMonth, startOfLastMonth, endOfLastMonth);

                // --- C. Ambil Nilai Odometer Paling Update ---
                // Perbaikan: Gunakan WaktuMulai. Jika kosong, fallback ke Odometer bawaan mobil
                var currentOdo = trips.Where(t => t.VehicleId == vehicle.Id)
                                    .OrderByDescending(t => t.WaktuMulai)
                                    .Select(t => t.OdometerAkhir)
                                    .FirstOrDefault() ?? vehicle.Odometer;

                // Perbaikan: Pakai .Tipe dan .TipeBensin (dengan .ToString() karena Enum)
                string tipe = vehicle.Tipe ?? "General";
                string fuelStr = vehicle.TipeBensin.ToString();

                if (calc.TotalBensinCurrent > 0)
                {
                    currentMonthScores.Add(new Tuple<string, double>(tipe, calc.SkorCurrent));
                }

                rawCalculations.Add(new
                {
                    VehicleId = vehicle.Id,
                    VehicleName = vehicle.VehicleName,
                    PlatNomor = vehicle.PlatNomor,
                    VehicleType = tipe,
                    FuelType = fuelStr,
                    CurrentOdometer = currentOdo,
                    TotalJarak = calc.TotalJarakCurrent,
                    TotalBensin = calc.TotalBensinCurrent,
                    SkorEfisiensi = calc.SkorCurrent,
                    VsBulanLalu = calc.VsBulanLalu
                });
            }

            // 3. Hitung Rata-Rata Skor Per Tipe Kendaraan Khusus Bulan Ini
            var averagesByType = currentMonthScores
                .GroupBy(x => x.Item1)
                .ToDictionary(g => g.Key, g => g.Average(x => x.Item2));

            // 4. Bandingkan Skor Kendaraan dengan Rata-Rata Tipenya
            var result = new List<VehicleEfficiencyDTO>();
            foreach (var item in rawCalculations)
            {
                averagesByType.TryGetValue(item.VehicleType, out double rataRataTipe);

                string status = "BELUM ADA DATA";
                if (item.TotalBensin > 0)
                {
                    status = item.SkorEfisiensi >= rataRataTipe ? "EFISIEN" : "BOROS";
                }

                result.Add(new VehicleEfficiencyDTO
                {
                    TotalJarak = item.TotalJarak,
                    TotalBensin = item.TotalBensin,
                    SkorEfisiensi = Math.Round(item.SkorEfisiensi, 1),
                    VsBulanLalu = Math.Round(item.VsBulanLalu, 1),
                    StatusEfisiensi = status
                });
            }

            return result;
        }

        public async Task<List<TripFuelRecordDTO>> GetFuelHistoryByDriverAsync(int driverId)
        {
            var bbmHistory = await _context.Fuels
                .Include(f => f.FuelPrice)
                .Include(f => f.Vehicle)
                .Where(f => f.DriverId == driverId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return bbmHistory.Select(_mapper.ToFuelRecordDto).ToList();
        }

        public async Task<List<TripFuelRecordDetailDTO>> GetFuelDetailByIdAsync(int fuelId)
        {
            var bbmHistory = await _context.Fuels
                .Include(f => f.FuelPrice)
                .Include(f => f.Vehicle)
                .Where(f => f.Id == fuelId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return bbmHistory.Select(_mapper.ToFuelRecordDetailDto).ToList();
        }

        public async Task<List<TripFuelRecordDTO>> GetFuelHistoryByUserIdAsync(int userId)
        {
            var driverId = await _context.Drivers
                .Where(d => d.UserId == userId)
                .Select(d => d.Id)
                .FirstOrDefaultAsync();

            if (driverId == 0)
            {
                throw new KeyNotFoundException("Driver tidak ditemukan untuk User ID tersebut.");
            }

            return await GetFuelHistoryByDriverAsync(driverId);
        }

        /// <summary>
        /// Membuat Point lokasi pengisian BBM (SRID 4326).
        /// Mengembalikan null bila GPS tidak tersedia, supaya pencatatan tetap bisa disimpan.
        /// </summary>
        private static Point? CreatePoint(double? latitude, double? longitude)
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
    }
}
