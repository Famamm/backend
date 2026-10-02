using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace fleetops_backend.Application.Services
{
    public class DriverMaintenanceService : IDriverMaintenanceService
    {
        private readonly AppDbContext _context;
        private readonly MaintenanceReminderOptions _options;

        public DriverMaintenanceService(AppDbContext context, IOptions<MaintenanceReminderOptions> options)
        {
            _context = context;
            _options = options.Value;
        }

        public async Task<DriverMaintenanceStatusDTO> GetMaintenanceStatusAsync(int userId)
        {
            var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == userId)
                ?? throw new KeyNotFoundException("Driver tidak ditemukan.");

            var vehicle = await _context.Vehicles
                .Where(v => v.DriverId == driver.Id && v.OrganizationId == driver.OrganizationId)
                .OrderBy(v => v.Id)
                .FirstOrDefaultAsync()
                ?? throw new KeyNotFoundException("Anda belum ditugaskan ke kendaraan.");

            var maintenances = await _context.Maintenances
                .Where(m => m.VehicleId == vehicle.Id && m.OrganizationId == driver.OrganizationId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            var open = maintenances
                .Where(m => m.Status is MaintenanceStatus.Scheduled or MaintenanceStatus.InProgress)
                .ToList();
            var lastCompleted = maintenances
                .Where(m => m.Status == MaintenanceStatus.Completed)
                .OrderByDescending(m => m.Tanggal)
                .ThenByDescending(m => m.Id)
                .FirstOrDefault();

            return new DriverMaintenanceStatusDTO
            {
                Summary = MaintenanceMapper.ToVehicleStatus(vehicle, open, lastCompleted, today, _options),
                Maintenances = maintenances
                    .Select(m => MaintenanceMapper.ToDto(m, vehicle, today, _options))
                    .ToList()
            };
        }
    }
}
