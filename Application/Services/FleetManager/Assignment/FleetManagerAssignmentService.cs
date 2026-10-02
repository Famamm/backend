using Microsoft.EntityFrameworkCore;
using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class FleetManagerAssignmentService : IFleetManagerAssignmentService
    {
        private readonly AppDbContext _context;
        private readonly IAssignmentValidator _validator;
        private readonly IAssignmentMapper _mapper;

        public FleetManagerAssignmentService(
            AppDbContext context,
            IAssignmentValidator validator,
            IAssignmentMapper mapper)
        {
            _context = context;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<CurrentDriverDTO> AssignDriverAsync(int organizationId, AssignVehicleDTO dto)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.Driver)
                    .ThenInclude(d => d!.User)
                .FirstOrDefaultAsync(v => v.Id == dto.VehicleId && v.OrganizationId == organizationId);
            _validator.ValidateVehicleExists(vehicle);
            _validator.ValidateVehicleIsActive(vehicle!);

            var newDriver = await _context.Drivers
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == dto.DriverId && d.OrganizationId == organizationId);
            _validator.ValidateDriverExists(newDriver);
            _validator.ValidateDriverIsActive(newDriver!);

            await ValidateNobodyIsOnTripAsync(vehicle!.Driver, newDriver!);

            // Menugaskan driver baru otomatis melepas driver lama dari kendaraan
            // ini, dan melepas driver baru dari kendaraan yang sebelumnya ia pegang.
            await ReleaseDriverFromOtherVehiclesAsync(organizationId, dto.DriverId, dto.VehicleId);

            vehicle.DriverId = dto.DriverId;
            await _context.SaveChangesAsync();

            return await GetCurrentDriverAsync(organizationId, dto.VehicleId);
        }

        public async Task<CurrentDriverDTO> UnassignDriverAsync(int organizationId, int vehicleId)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.Driver)
                    .ThenInclude(d => d!.User)
                .FirstOrDefaultAsync(v => v.Id == vehicleId && v.OrganizationId == organizationId);
            _validator.ValidateVehicleExists(vehicle);

            if (vehicle!.Driver != null)
            {
                var onTrip = await HasOngoingTripAsync(vehicle.Driver.Id);
                _validator.ValidateDriverNotOnTrip(onTrip, DriverName(vehicle.Driver));
            }

            vehicle.DriverId = null;
            await _context.SaveChangesAsync();

            return await GetCurrentDriverAsync(organizationId, vehicleId);
        }

        public async Task<CurrentDriverDTO> GetCurrentDriverAsync(int organizationId, int vehicleId)
        {
            var vehicle = await _context.Vehicles
                .Include(v => v.Driver)
                    .ThenInclude(d => d!.User)
                .FirstOrDefaultAsync(v => v.Id == vehicleId && v.OrganizationId == organizationId);

            _validator.ValidateVehicleExists(vehicle);

            return _mapper.ToCurrentDriverDto(vehicle!);
        }

        public async Task<List<AvailableDriverDTO>> GetAvailableDriversAsync(int organizationId)
        {
            var assignedDriverIds = await _context.Vehicles
                .Where(v => v.OrganizationId == organizationId && v.DriverId != null)
                .Select(v => v.DriverId!.Value)
                .ToListAsync();

            var drivers = await _context.Drivers
                .Include(d => d.User)
                .Where(d => d.OrganizationId == organizationId
                    && d.Status != "Deleted"
                    && !assignedDriverIds.Contains(d.Id))
                .OrderBy(d => d.User!.FullName)
                .ToListAsync();

            return drivers.Select(_mapper.ToAvailableDriverDto).ToList();
        }

        private async Task ValidateNobodyIsOnTripAsync(Driver? currentDriver, Driver newDriver)
        {
            if (currentDriver != null && currentDriver.Id != newDriver.Id)
            {
                var currentOnTrip = await HasOngoingTripAsync(currentDriver.Id);
                _validator.ValidateDriverNotOnTrip(currentOnTrip, DriverName(currentDriver));
            }

            var newOnTrip = await HasOngoingTripAsync(newDriver.Id);
            _validator.ValidateDriverNotOnTrip(newOnTrip, DriverName(newDriver));
        }

        private async Task ReleaseDriverFromOtherVehiclesAsync(int organizationId, int driverId, int targetVehicleId)
        {
            var otherVehicles = await _context.Vehicles
                .Where(v => v.DriverId == driverId
                    && v.OrganizationId == organizationId
                    && v.Id != targetVehicleId)
                .ToListAsync();

            foreach (var other in otherVehicles)
                other.DriverId = null;
        }

        // Trip yang belum punya WaktuSelesai ikut dihitung sedang berjalan,
        // mengikuti pengecekan yang dipakai TripService saat memulai trip baru.
        private Task<bool> HasOngoingTripAsync(int driverId)
        {
            return _context.Trips
                .AnyAsync(t => t.DriverId == driverId
                    && (t.Status == "Ongoing" || t.WaktuSelesai == null));
        }

        private static string DriverName(Driver driver)
        {
            return driver.User?.FullName ?? "Driver";
        }
    }
}
