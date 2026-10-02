using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace fleetops_backend.Application.Services
{
    public class FleetManagerFuelService : IFleetManagerFuelService
    {
        private readonly AppDbContext _context;
        private readonly IFuelFillReader _fillReader;
        private readonly IFuelBaselineCalculator _baselineCalculator;
        private readonly FuelAnomalyOptions _options;

        public FleetManagerFuelService(
            AppDbContext context,
            IFuelFillReader fillReader,
            IFuelBaselineCalculator baselineCalculator,
            IOptions<FuelAnomalyOptions> options)
        {
            _context = context;
            _fillReader = fillReader;
            _baselineCalculator = baselineCalculator;
            _options = options.Value;
        }

        public async Task<FuelBaselineDTO> GetVehicleBaselineAsync(int organizationId, int vehicleId)
        {
            var vehicleExists = await _context.Vehicles
                .AnyAsync(v => v.Id == vehicleId && v.OrganizationId == organizationId);

            if (!vehicleExists)
            {
                throw new KeyNotFoundException("Kendaraan tidak ditemukan.");
            }

            var fills = await _fillReader.GetLatestAsync(vehicleId, _options.Window + 1);

            return _baselineCalculator.Calculate(fills);
        }
    }
}
