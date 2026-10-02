using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class DriverDashboardService : IDriverDashboardService
    {
        private readonly AppDbContext _context;
        private readonly IDriverFuelService _fuelService;

        public DriverDashboardService(AppDbContext context, IDriverFuelService fuelService)
        {
            _context = context;
            _fuelService = fuelService;
        }

        public async Task<DriverDashboardDTO> GetDashboardDriverAsync(int driverId)
        {
            var driver = await _context.Drivers
                    .Include(d => d.User)
                    .Include(d => d.Organization)
                    .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null)
                throw new KeyNotFoundException("Driver not found.");

            var organizationId = driver.OrganizationId;

            var vehicle = await _context.Vehicles
                    .Where(v => v.DriverId == driverId && v.OrganizationId == organizationId)
                    .OrderByDescending(v => v.Id)
                    .FirstOrDefaultAsync();

            var fuelEfficiency = vehicle != null ? await _fuelService.GetVehicleEfficiencyByVehicleIdAsync(vehicle.Id) : null;

            var totalJarak = await _context.Trips
                    .Where(t => t.DriverId == driverId && t.OrganizationId == organizationId)
                    .SelectMany(t => t.Detail)
                    .SumAsync(td => (int?)td.Jarak) ?? 0;

            var tripSelesai = await _context.Trips
                    .CountAsync(t => t.DriverId == driverId && t.OrganizationId == organizationId && t.Status == "Finished");

            var result = new DriverDashboardDTO
                {
                    DriverName = driver.User?.FullName ?? "Unknown",
                    TotalJarak = totalJarak,
                    TripSelesai = tripSelesai,
                    VehicleName = vehicle?.VehicleName ?? "-",
                    PlatNomor = vehicle?.PlatNomor ?? "-",
                    VehicleStatus = vehicle?.Status ?? "Tidak Tersedia",
                    OrganizationName = driver.Organization?.OrganizationName ?? "Unknown",
                    FuelEfficiency = fuelEfficiency
                };

            return result;
        }

        public async Task<DriverDashboardDTO> GetDashboardDriverByUserIdAsync(int userId)
        {
            var driver = await _context.Drivers
                .Include(d => d.User)
                .Include(d => d.Organization)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (driver == null)
            {
                throw new KeyNotFoundException("Driver not found for the given user ID.");
            }

            return await GetDashboardDriverAsync(driver.Id);
        }
    }
}