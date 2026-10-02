using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class DriverDamageReportService : IDriverDamageReportService
    {
        private const string OngoingTripStatus = "Ongoing";

        private readonly AppDbContext _context;
        private readonly IDamageReportValidator _validator;
        private readonly IImageService _imageService;
        private readonly ITripDamageReportNotifier _notifier;

        public DriverDamageReportService(
            AppDbContext context,
            IDamageReportValidator validator,
            IImageService imageService,
            ITripDamageReportNotifier notifier)
        {
            _context = context;
            _validator = validator;
            _imageService = imageService;
            _notifier = notifier;
        }

        public async Task<DamageReportDTO> CreateAsync(int userId, int tripId, CreateDamageReportDTO dto)
        {
            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (driver == null)
            {
                throw new KeyNotFoundException("Driver tidak ditemukan.");
            }

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t => t.Id == tripId && t.DriverId == driver.Id);

            if (trip == null)
            {
                throw new KeyNotFoundException("Trip tidak ditemukan atau bukan milik driver.");
            }

            if (trip.Status != OngoingTripStatus)
            {
                throw new InvalidOperationException("Laporan kerusakan hanya dapat dibuat saat trip berjalan.");
            }

            _validator.ValidateCreate(dto);

            var severity = _validator.ParseSeverity(dto.Severity);
            var operationalStatus = _validator.ParseOperationalStatus(dto.OperationalStatus);

            var location = dto.Location;
            if (string.IsNullOrWhiteSpace(location))
            {
                location = await ResolveLocationAsync(trip);
            }

            string? imageUrl = null;
            if (dto.Image != null && dto.Image.Length > 0)
            {
                imageUrl = await _imageService.UploadImageAsync(dto.Image, "fleetops/damage-reports");
            }

            var report = new DamageReport
            {
                TripId = trip.Id,
                DriverId = driver.Id,
                VehicleId = trip.VehicleId,
                OrganizationId = trip.OrganizationId,
                Severity = severity,
                Priority = _validator.DerivePriority(severity, operationalStatus),
                OperationalStatus = operationalStatus,
                Status = DamageReportStatus.Reported,
                DamageType = dto.DamageType?.Trim(),
                Note = dto.Note?.Trim(),
                IncidentAt = DateTimeOffset.UtcNow,
                Location = location,
                Odometer = dto.Odometer ?? trip.OdometerAwal,
                ImageUrlPath = imageUrl,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _context.DamageReports.Add(report);
            await _context.SaveChangesAsync();

            await _notifier.NotifyNewReportAsync(report);

            var created = await LoadReportAsync(report.Id);
            return DamageReportMapper.ToDto(created);
        }

        public async Task<List<DamageReportDTO>> GetByTripAsync(int userId, int tripId)
        {
            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (driver == null)
            {
                throw new KeyNotFoundException("Driver tidak ditemukan.");
            }

            var tripExists = await _context.Trips
                .AnyAsync(t => t.Id == tripId && t.DriverId == driver.Id);

            if (!tripExists)
            {
                throw new KeyNotFoundException("Trip tidak ditemukan atau bukan milik driver.");
            }

            var reports = await _context.DamageReports
                .Include(r => r.Vehicle)
                .Include(r => r.Driver)
                    .ThenInclude(d => d!.User)
                .Where(r => r.TripId == tripId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reports.Select(DamageReportMapper.ToDto).ToList();
        }

        private async Task<string?> ResolveLocationAsync(Trip trip)
        {
            var lastPoint = await _context.TripGpsPoints
                .Where(p => p.TripId == trip.Id)
                .OrderByDescending(p => p.CapturedAt)
                .FirstOrDefaultAsync();

            if (lastPoint != null)
            {
                return $"{lastPoint.Coordinate.Y}, {lastPoint.Coordinate.X}";
            }

            return trip.LokasiAwal;
        }

        private async Task<DamageReport> LoadReportAsync(int id)
        {
            return await _context.DamageReports
                .Include(r => r.Vehicle)
                .Include(r => r.Driver)
                    .ThenInclude(d => d!.User)
                .FirstAsync(r => r.Id == id);
        }
    }
}
