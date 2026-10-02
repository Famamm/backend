using Microsoft.EntityFrameworkCore;
using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;

namespace fleetops_backend.Application.Services
{
    public class FleetManagerVehicleService : IFleetManagerVehicleService
    {
        private const string ActiveStatus = "Active";
        private const string DeletedStatus = "Deleted";

        private readonly AppDbContext _context;

        public FleetManagerVehicleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<FleetDashboardDTO> GetSummaryAsync(int organizationId)
        {
            var vehicles = await _context.Vehicles
                .Where(v => v.OrganizationId == organizationId)
                .Select(v => new { v.Status, v.DriverId })
                .ToListAsync();

            var activeDrivers = await _context.Drivers
                .Where(d => d.OrganizationId == organizationId && d.Status != DeletedStatus)
                .Select(d => d.Id)
                .ToListAsync();

            var assignedDriverIds = vehicles
                .Where(v => v.DriverId.HasValue)
                .Select(v => v.DriverId!.Value)
                .ToHashSet();

            return new FleetDashboardDTO
            {
                TotalKendaraan = vehicles.Count,
                KendaraanAktif = vehicles.Count(v => v.Status == ActiveStatus),
                KendaraanBelumAdaDriver = vehicles.Count(v => v.DriverId == null),
                TotalDriver = activeDrivers.Count,
                DriverBelumDitugaskan = activeDrivers.Count(id => !assignedDriverIds.Contains(id))
            };
        }

        public async Task<List<FleetVehicleDTO>> GetVehiclesAsync(int organizationId)
        {
            return await _context.Vehicles
                .Include(v => v.Driver)
                    .ThenInclude(d => d!.User)
                .Where(v => v.OrganizationId == organizationId)
                .OrderBy(v => v.VehicleName)
                .Select(v => new FleetVehicleDTO
                {
                    VehicleId = v.Id,
                    VehicleName = v.VehicleName,
                    PlatNomor = v.PlatNomor,
                    Tipe = v.Tipe,
                    Status = v.Status,
                    Odometer = v.Odometer,
                    ImageUrl = v.ImageUrl,
                    DriverId = v.DriverId,
                    NamaDriver = v.Driver != null ? v.Driver.User!.FullName : null
                })
                .ToListAsync();
        }
    }
}
