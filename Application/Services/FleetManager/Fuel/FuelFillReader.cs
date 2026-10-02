using fleetops_backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class FuelFillReader : IFuelFillReader
    {
        private readonly AppDbContext _context;

        public FuelFillReader(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<FuelFill>> GetLatestAsync(int vehicleId, int count)
        {
            var fills = await _context.Fuels
                .Where(f => f.VehicleId == vehicleId)
                .OrderByDescending(f => f.CreatedAt)
                .ThenByDescending(f => f.Id)
                .Take(count)
                .Select(f => new FuelFill(f.OdometerIsi, f.JmlLiter))
                .ToListAsync();

            fills.Reverse();

            return fills;
        }
    }
}
