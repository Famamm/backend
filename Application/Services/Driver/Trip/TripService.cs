using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;


namespace fleetops_backend.Application.Services
{
    public class TripService : ITripService
    {
        
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ITripValidator _tripValidator;
        private readonly ITripCalculator _tripCalculator;

        private readonly ITripMapper _tripMapper;
        private readonly IImageService _imageService;
        private readonly IMaintenanceReminderService _maintenanceReminderService;

        public TripService(
            AppDbContext context, 
            IHttpContextAccessor httpContextAccessor,
            ITripValidator tripValidator,
            ITripCalculator tripCalculator,
            ITripMapper tripMapper,
            IImageService imageService,
            IMaintenanceReminderService maintenanceReminderService
            )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _tripValidator = tripValidator;
            _tripCalculator = tripCalculator;
            _tripMapper = tripMapper;
            _imageService = imageService;
            _maintenanceReminderService = maintenanceReminderService;
        }

        public async Task<Trip?> StartTripAsync(int userId, StartTripDTO dto)
        {
            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d => d.UserId == userId);
            if (driver == null)
            {
                throw new KeyNotFoundException("Driver not found for the given user ID.");
            }

            var driverId = driver.Id;
            _tripValidator.ValidateDriverForTrip(driverId, dto.DriverId);
            var orgClaimId = ClaimsHelper.GetOrganizationIdOrNull(_httpContextAccessor.HttpContext?.User);
            if (orgClaimId.HasValue && driver.OrganizationId != orgClaimId.Value)
            {
                throw new InvalidOperationException("Unauthorized organization.");
            }
            
            // Check if driver already has an active trip
            var activeTrip = await _context.Trips
                .FirstOrDefaultAsync(
                    t => t.DriverId == driverId &&
                    t.OrganizationId == orgClaimId &&
                    (t.Status == "Ongoing" || t.WaktuSelesai == null)
                );

            _tripValidator.ValidateNoActiveTrip(activeTrip);
            
            var dateRange = _tripCalculator.GetInspectionDateRange(DateTimeOffset.UtcNow);
            
            // Check if driver has completed inspection for today
            var todayInspection = await _context.Set<Inspection>()
                .FirstOrDefaultAsync(
                    i => i.DriverId == driverId &&
                    i.OrganizationId == orgClaimId &&
                    i.CreatedAt >= dateRange.StartUtc &&
                    i.CreatedAt < dateRange.EndUtc &&
                    i.Status == true
                );

            if (todayInspection == null)
            {
                throw new InvalidOperationException("Driver must complete an inspection before starting a trip.");
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(v => v.Id == dto.VehicleId);
            if (vehicle == null)
            {
                throw new InvalidOperationException("Vehicle must be filled");
            }
            _tripValidator.ValidateVehicleForTrip(vehicle,driverId,orgClaimId);

            var startPoint = _tripCalculator.CreatePoint(dto.KordAwalLatitude,dto.KordAwalLongitude);

            var trip = new Trip
            {
                DriverId = driverId,
                VehicleId = dto.VehicleId,
                OrganizationId = vehicle.OrganizationId,
                LokasiAwal = dto.LokasiAwal,
                LokasiAkhir = dto.LokasiAkhir,
                KordAwal = startPoint,
                OdometerAwal = dto.OdometerAwal ?? vehicle.Odometer,
                WaktuMulai = DateTimeOffset.UtcNow,
                Status = "Ongoing"
            };

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            var tripDetail = new TripDetail
            {
                TripId = trip.Id,
                Jarak = 0,
                AvgSpeed = 0,
                Waktu = 0
            };

            _context.Set<TripDetail>().Add(tripDetail);
            await _context.SaveChangesAsync();

            return trip;
        }

        public async Task<Trip> StopTripAsync(
            int tripId,
            int userId,
            StopTripDTO dto)
        {
            if (dto is null)
            { 
                throw new ArgumentNullException(nameof(dto));
            }

            if (dto.Image is null || dto.Image.Length == 0)
            {
                throw new InvalidOperationException("Image is required.");
            }

            var image = dto.Image;

            var orgClaimId = ClaimsHelper.GetOrganizationIdOrNull(_httpContextAccessor.HttpContext?.User);

            var trip = await _context.Trips
                .Include(t => t.Detail)
                .FirstOrDefaultAsync(t => t.Id == tripId);
            if (trip == null)
            {
                throw new KeyNotFoundException("Trip not found.");
            }

            if (orgClaimId.HasValue && trip.OrganizationId != orgClaimId.Value)
            {
                throw new InvalidOperationException("Unauthorized organization.");
            }

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (driver == null)
            {
                throw new KeyNotFoundException("Driver not found for the authenticated user.");
            }
           
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == dto.VehicleId);
            if (vehicle == null)
            {
                throw new KeyNotFoundException("Vehicle not found.");
            }

            _tripValidator.ValidateStopTrip(trip,driver,vehicle,dto);
            _tripValidator.ValidateOdometer(dto.OdometerAkhir,trip.OdometerAwal);

            var endPoint = _tripCalculator.CreatePoint(dto.KordAkhirLatitude,dto.KordAkhirLongitude);

            trip.KordAkhir = endPoint;
            trip.LokasiAkhir = dto.LokasiAkhir;
            trip.Catatan = dto.Catatan;
            trip.OdometerAkhir = dto.OdometerAkhir;
            trip.WaktuSelesai = DateTimeOffset.UtcNow;
            trip.Status = "Finished";

            if (trip.OdometerAkhir.HasValue)
            {
                vehicle.Odometer = trip.OdometerAkhir.Value;
            }

            // Update TripDetail with calculated metrics
            var tripDetail = trip.Detail.FirstOrDefault();

            if (tripDetail == null)
            {
                tripDetail = new TripDetail
                {
                    TripId = tripId
                };

                _context.Set<TripDetail>().Add(tripDetail);
            }

            _tripCalculator.CalculateMetrics(
                tripDetail,
                trip.OdometerAwal,
                trip.OdometerAkhir,
                trip.WaktuMulai,
                trip.WaktuSelesai.Value);

            var imageUrl = await _imageService.UploadImageAsync(image);

            tripDetail.ImageUrl = imageUrl;

            await _context.SaveChangesAsync();
            await _maintenanceReminderService.CheckVehicleAsync(vehicle.Id);

            return trip;
        }

        public async Task<Trip> CancelTripAsync(int tripId, int userId)
        {
            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (driver == null)
            {
                throw new KeyNotFoundException(
                    "Driver not found for the authenticated user.");
            }

            var orgClaimId = ClaimsHelper.GetOrganizationIdOrNull(_httpContextAccessor.HttpContext?.User);

            var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == tripId);
            if (trip == null)
            {
                throw new KeyNotFoundException("Trip not found.");
            }

            if (orgClaimId.HasValue && trip.OrganizationId != orgClaimId.Value)
            {
                throw new InvalidOperationException("Unauthorized organization.");
            }

            _tripValidator.ValidateCancelTrip(trip,driver,driver.Id);

            trip.Status = "Canceled";
            trip.WaktuSelesai = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return trip;
        }

        public async Task<List<GetTripDTO>> GetTripsByDriverAsync(int driverId)
        {
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == driverId);
            if (driver == null)
            {
                throw new KeyNotFoundException("Driver not found.");
            }

            var orgClaimId = ClaimsHelper.GetOrganizationIdOrNull(_httpContextAccessor.HttpContext?.User);
            if (orgClaimId.HasValue && driver.OrganizationId != orgClaimId.Value)
            {
                throw new InvalidOperationException("Unauthorized organization.");
            }

            var trips = await _context.Trips
                .Where(t => t.DriverId == driverId && t.OrganizationId == driver.OrganizationId)
                .Include(t => t.Detail)
                .ToListAsync();

            var tripDtos = trips
                .Select(trip => _tripMapper.ToTripDto(
                    trip,
                    trip.Detail.FirstOrDefault()))
                .ToList();

            return tripDtos;
        }

        public async Task<GetDetailTripDTO?> GetDetailTripAsync(int tripId)
        {
            var orgClaimId = ClaimsHelper.GetOrganizationIdOrNull(_httpContextAccessor.HttpContext?.User);

            var trip = await _context.Trips
                .Include(t => t.Detail)
                .Include(t => t.Driver)
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(t => t.Id == tripId);

            if (trip == null) return null;
            if (orgClaimId.HasValue && trip.OrganizationId != orgClaimId.Value) return null;

            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == trip.DriverId);
            var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == trip.VehicleId);

            if (driver == null || vehicle == null) return null;
            if (driver.OrganizationId != trip.OrganizationId || vehicle.OrganizationId != trip.OrganizationId) return null;

            return _tripMapper.ToDetailTripDto(trip,driver,vehicle);
        }

        public async Task<GetActiveTripDTO?> GetActiveTripAsync(int driverId)
        {
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.Id == driverId);
            if (driver == null)
                throw new KeyNotFoundException($"Driver dengan ID {driverId} tidak ditemukan.");

            var orgClaimId = ClaimsHelper.GetOrganizationIdOrNull(_httpContextAccessor.HttpContext?.User);
            if (orgClaimId.HasValue && driver.OrganizationId != orgClaimId.Value)
                throw new InvalidOperationException("Unauthorized organization.");

            var trip = await _context.Trips
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(
                    t => t.DriverId == driverId &&
                    t.OrganizationId == driver.OrganizationId &&
                    t.Status == "Ongoing");

            if (trip == null) return null;

            return _tripMapper.ToActiveTripDto(trip,DateTimeOffset.UtcNow);
        }

    }
}