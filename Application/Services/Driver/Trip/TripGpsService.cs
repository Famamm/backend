using fleetops_backend.Application.DTOs;
using fleetops_backend.Common;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace fleetops_backend.Application.Services
{
    public class TripGpsService : ITripGpsService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TripGpsService(
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<SaveTripGpsResultDTO> SaveGpsPointsAsync(
            int tripId,
            int userId,
            SaveTripGpsDTO dto)
        {
            if (dto == null || dto.Points.Count == 0)
            {
                throw new InvalidOperationException(
                    "Minimal satu titik GPS harus dikirim.");
            }

            if (dto.Points.Count > 500)
            {
                throw new InvalidOperationException(
                    "Maksimal 500 titik GPS per batch.");
            }

            var driver = await _context.Drivers
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (driver == null)
            {
                throw new KeyNotFoundException(
                    "Driver tidak ditemukan.");
            }

            var organizationId =
                ClaimsHelper.GetOrganizationIdOrNull(
                    _httpContextAccessor.HttpContext?.User);

            var trip = await _context.Trips
                .FirstOrDefaultAsync(t =>
                    t.Id == tripId &&
                    t.DriverId == driver.Id &&
                    (!organizationId.HasValue ||
                     t.OrganizationId == organizationId.Value));

            if (trip == null)
            {
                throw new KeyNotFoundException(
                    "Trip tidak ditemukan atau bukan milik driver.");
            }

            if (trip.Status != "Ongoing")
            {
                throw new InvalidOperationException(
                    "GPS hanya dapat disimpan saat trip berjalan.");
            }

            if (dto.Points.Any(point =>
                point.ClientId == Guid.Empty ||
                point.Latitude < -90 ||
                point.Latitude > 90 ||
                point.Longitude < -180 ||
                point.Longitude > 180))
            {
                throw new InvalidOperationException(
                    "Koordinat GPS tidak valid.");
            }

            var receivedCount = dto.Points.Count;
            var points = dto.Points
                .GroupBy(point => point.ClientId)
                .Select(group => group.First())
                .ToList();

            var clientIds = points
                .Select(point => point.ClientId)
                .ToList();

            var existingClientIds = await _context.TripGpsPoints
                .Where(point =>
                    point.TripId == tripId &&
                    clientIds.Contains(point.ClientId))
                .Select(point => point.ClientId)
                .ToListAsync();

            var newGpsPoints = points
                .Where(point =>
                    !existingClientIds.Contains(point.ClientId))
                .Select(point => new TripGpsPoint
                {
                    TripId = tripId,
                    ClientId = point.ClientId,
                    Coordinate = new Point(
                        point.Longitude,
                        point.Latitude)
                    {
                        SRID = 4326
                    },
                    CapturedAt = point.CapturedAt,
                    AccuracyMeters = point.AccuracyMeters
                })
                .ToList();

            if (newGpsPoints.Count == 0)
            {
                return new SaveTripGpsResultDTO
                {
                    ReceivedCount = receivedCount,
                    SavedCount = 0,
                    DuplicateCount = receivedCount
                };
            }

            _context.TripGpsPoints.AddRange(newGpsPoints);
            await _context.SaveChangesAsync();

            return new SaveTripGpsResultDTO
            {
                ReceivedCount = receivedCount,
                SavedCount = newGpsPoints.Count,
                DuplicateCount = receivedCount - newGpsPoints.Count
            };
        }
    }
}