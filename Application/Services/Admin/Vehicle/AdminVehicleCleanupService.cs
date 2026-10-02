using fleetops_backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class AdminVehicleCleanupService : IAdminVehicleCleanupService
    {
        private readonly AppDbContext _context;

        public AdminVehicleCleanupService(AppDbContext context)
        {
            _context = context;
        }

        public async Task DeleteVehicleDependenciesAsync(int vehicleId)
        {
            var trips = await _context.Trips
                .Where(t => t.VehicleId == vehicleId)
                .ToListAsync();
            _context.Trips.RemoveRange(trips);

            var inspections = await _context.Inspections
                .Where(i => i.VehicleId == vehicleId)
                .ToListAsync();
            _context.Inspections.RemoveRange(inspections);

            var fuels = await _context.Fuels
                .Where(f => f.VehicleId == vehicleId)
                .ToListAsync();
            _context.Fuels.RemoveRange(fuels);

            var maintenances = await _context.Maintenances
                .Where(m => m.VehicleId == vehicleId)
                .ToListAsync();
            _context.Maintenances.RemoveRange(maintenances);
        }
    }
}
