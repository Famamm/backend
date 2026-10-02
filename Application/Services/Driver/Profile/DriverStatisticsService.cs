using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class DriverStatisticsService : IDriverStatisticsService
    {
        private const string FinishedStatus = "Finished";

        private readonly AppDbContext _context;

        public DriverStatisticsService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DriverStatisticsDto> GetStatisticsAsync(int driverId)
        {
            var finishedTrips = _context.Trips
                .Where(t => t.DriverId == driverId && t.Status == FinishedStatus);

            var totalTrip = await finishedTrips.CountAsync();

            var totals = await finishedTrips
                .SelectMany(t => t.Detail)
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Jarak = g.Sum(d => (long)d.Jarak),
                    Waktu = g.Sum(d => (long)d.Waktu)
                })
                .FirstOrDefaultAsync();

            return DriverStatisticsCalculator.Calculate(totalTrip, totals?.Jarak ?? 0, totals?.Waktu ?? 0);
        }

        public async Task<DriverStatisticsDto> GetStatisticsByUserIdAsync(int userId)
        {
            var driverId = await _context.Drivers
                .Where(d => d.UserId == userId)
                .Select(d => (int?)d.Id)
                .FirstOrDefaultAsync();

            if (driverId == null)
            {
                throw new KeyNotFoundException($"Driver untuk User ID {userId} tidak ditemukan");
            }

            return await GetStatisticsAsync(driverId.Value);
        }
    }
}
