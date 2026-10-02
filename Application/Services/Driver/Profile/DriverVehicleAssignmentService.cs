using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class DriverVehicleAssignmentService : IDriverVehicleAssignmentService
    {
        private readonly AppDbContext _context;

        public DriverVehicleAssignmentService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DriverVehicleDto>> GetAvailableVehiclesAsync(int organizationId, int driverId)
        {
            return await _context.Vehicles
                .Where(v => v.OrganizationId == organizationId
                    && v.Status == "Active"
                    && (v.DriverId == null || v.DriverId == driverId))
                .Select(v => new DriverVehicleDto
                {
                    VehicleId = v.Id,
                    VehicleName = v.VehicleName,
                    PlatNomor = v.PlatNomor,
                    Tipe = v.Tipe,
                    TipeBensin = v.TipeBensin,
                    Status = v.Status,
                    ImageUrl = v.ImageUrl,
                    Odometer = v.Odometer
                })
                .ToListAsync();
        }

    }
}
