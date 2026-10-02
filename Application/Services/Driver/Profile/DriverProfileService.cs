using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class DriverProfileService : IDriverProfileService
    {
        private readonly AppDbContext _context;
        private readonly IDriverStatisticsService _statisticsService;
        private readonly IDriverVehicleAssignmentService _vehicleAssignmentService;
        private readonly IDriverProfileMapper _mapper;

        public DriverProfileService(
            AppDbContext context,
            IDriverStatisticsService statisticsService,
            IDriverVehicleAssignmentService vehicleAssignmentService,
            IDriverProfileMapper mapper)
        {
            _context = context;
            _statisticsService = statisticsService;
            _vehicleAssignmentService = vehicleAssignmentService;
            _mapper = mapper;
        }

        public async Task<GetDriverProfileDTO> GetDriverProfileAsync(int driverId)
        {
            var driver = await _context.Drivers
                .Include(d => d.Organization)
                .Include(d => d.User)
                .Include(d => d.Vehicle)
                .Include(d => d.Trip)
                .FirstOrDefaultAsync(d => d.Id == driverId);

            if (driver == null || driver.User == null)
            {
                throw new KeyNotFoundException($"Driver dengan ID {driverId} tidak ditemukan");
            }

            var statistics = await _statisticsService.GetStatisticsAsync(driverId);
            var activeVehicle = _mapper.MapVehicle(driver.Vehicle?.FirstOrDefault(v => v.Status == "Active"));
            var kendaraanTersedia = await _vehicleAssignmentService.GetAvailableVehiclesAsync(driver.OrganizationId, driverId);

            return _mapper.MapProfile(driver, statistics, activeVehicle, kendaraanTersedia);
        }

        public async Task<GetDriverProfileDTO> GetDriverProfileByUserIdAsync(int userId)
        {
            var driverId = await _context.Drivers
                .Where(d => d.UserId == userId)
                .Select(d => (int?)d.Id)
                .FirstOrDefaultAsync();

            if (driverId == null)
            {
                throw new KeyNotFoundException($"Driver untuk User ID {userId} tidak ditemukan");
            }

            return await GetDriverProfileAsync(driverId.Value);
        }

    }
}
