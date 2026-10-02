using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class TripRouteService : ITripRouteService
    {
        private const string OngoingStatus = "Ongoing";

        private readonly AppDbContext _context;

        public TripRouteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TripRouteDTO> GetTripRouteAsync(int organizationId, int tripId)
        {
            var trip = await _context.Trips
                .Include(t => t.Driver).ThenInclude(d => d!.User)
                .Include(t => t.Vehicle)
                .FirstOrDefaultAsync(t =>
                    t.Id == tripId && t.OrganizationId == organizationId);

            if (trip == null)
            {
                throw new KeyNotFoundException("Trip tidak ditemukan.");
            }

            var points = await _context.TripGpsPoints
                .Where(point => point.TripId == tripId)
                .OrderBy(point => point.CapturedAt)
                .ToListAsync();

            return TripRouteMapper.ToRouteDto(trip, points);
        }

        public async Task<List<DriverLiveLocationDTO>> GetLiveLocationsAsync(int organizationId)
        {
            var ongoingTrips = await _context.Trips
                .Include(t => t.Driver).ThenInclude(d => d!.User)
                .Include(t => t.Vehicle)
                .Where(t => t.OrganizationId == organizationId
                    && t.Status == OngoingStatus)
                .OrderBy(t => t.WaktuMulai)
                .ToListAsync();

            if (ongoingTrips.Count == 0)
            {
                return new List<DriverLiveLocationDTO>();
            }

            var lastPointByTrip = await GetLastPointPerTripAsync(
                ongoingTrips.Select(t => t.Id).ToList());

            return ongoingTrips
                .Select(trip => TripRouteMapper.ToLiveLocationDto(
                    trip,
                    lastPointByTrip.GetValueOrDefault(trip.Id)))
                .ToList();
        }

        // Seluruh titik trip yang sedang berjalan diambil dalam satu query lalu
        // dikelompokkan di memori. Query per trip akan menjadi N+1 dan jumlah
        // trip aktif ikut bertambah seiring armada bertambah.
        private async Task<Dictionary<int, TripGpsPoint>> GetLastPointPerTripAsync(
            List<int> tripIds)
        {
            var points = await _context.TripGpsPoints
                .Where(point => tripIds.Contains(point.TripId))
                .ToListAsync();

            return points
                .GroupBy(point => point.TripId)
                .ToDictionary(
                    group => group.Key,
                    group => group.OrderByDescending(point => point.CapturedAt).First());
        }
    }
}
