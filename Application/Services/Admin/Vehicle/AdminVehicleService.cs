using fleetops_backend.Application.DTOs;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
	public class AdminVehicleService : IAdminVehicleService
	{
		private readonly AppDbContext _context;
		private readonly IImageService _imageService;
		private readonly IDriverFuelService _fuelService;
		private readonly IAdminVehicleMapper _mapper;
		private readonly IAdminVehicleCleanupService _cleanupService;

		public AdminVehicleService(
			AppDbContext context,
			IImageService imageService,
			IDriverFuelService fuelService,
			IAdminVehicleMapper mapper,
			IAdminVehicleCleanupService cleanupService)
		{
			_context = context;
			_imageService = imageService;
			_fuelService = fuelService;
			_mapper = mapper;
			_cleanupService = cleanupService;
		}

		public async Task<Vehicle> CreateVehicleAsync(int organizationId, VehicleDTO dto)
		{
			var organizationExists = await _context.Organizations.FindAsync(organizationId);

			if (organizationExists == null)
			{
				throw new KeyNotFoundException("Organization not found.");
			}

			string? imageUrl = null;

			if (dto.Image != null && dto.Image.Length > 0)
			{
				imageUrl = await _imageService.UploadImageAsync(dto.Image, "fleetops/vehicles");
			}

			var vehicle = _mapper.ToVehicle(dto, organizationId, imageUrl ?? "");

			_context.Vehicles.Add(vehicle);
			await _context.SaveChangesAsync();

			return vehicle;
		}

		public async Task<GetVehicleDTO> GetVehicleByIdAsync(int organizationId, int vehicleId)
		{
			var vehicle = await _context.Vehicles.FindAsync(vehicleId);

			if (vehicle == null || vehicle.OrganizationId != organizationId)
				throw new KeyNotFoundException("Vehicle not found.");

			var vehicleEfficiency = await _fuelService.GetVehicleEfficiencyByVehicleIdAsync(vehicleId);

			return _mapper.ToVehicleDto(vehicle, vehicleEfficiency);
		}

		public async Task<Vehicle> DeleteVehicleAsync(int organizationId, int vehicleId)
		{
			var vehicle = await _context.Vehicles.FindAsync(vehicleId);

			if (vehicle == null || vehicle.OrganizationId != organizationId)
			{
				throw new KeyNotFoundException("Vehicle not found.");
			}

			await _cleanupService.DeleteVehicleDependenciesAsync(vehicleId);

			_context.Vehicles.Remove(vehicle);
			await _context.SaveChangesAsync();

			return vehicle;
		}

		public async Task<Vehicle> UpdateVehicleAsync(int organizationId, int vehicleId, UpdateVehicleDTO dto)
		{
			var vehicle = await _context.Vehicles.FindAsync(vehicleId);
			if (vehicle == null || vehicle.OrganizationId != organizationId)
				throw new KeyNotFoundException("Vehicle not found.");

			_mapper.ApplyUpdate(vehicle, dto);

			if (dto.Image != null && dto.Image.Length > 0)
			{
				vehicle.ImageUrl = await _imageService.UploadImageAsync(dto.Image, "fleetops/vehicles");
			}

			await _context.SaveChangesAsync();
			return vehicle;
		}

		public async Task<List<GetVehicleDTO>> GetVehiclesByOrganizationAsync(int organizationId)
		{
			var vehicles = await _context.Vehicles
				.Where(v => v.OrganizationId == organizationId)
				.ToListAsync();

			return vehicles.Select(_mapper.ToSummaryDto).ToList();
		}
	}
}
