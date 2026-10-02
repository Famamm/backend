using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fleetops_backend.Presentation.Controllers
{
	[ApiController]
	[Route("api/admin/vehicles")]
	[Authorize(Roles ="Admin")]
	public class AdminVehicleController : ControllerBase
	{
		private readonly IAdminVehicleService _adminVehicleService;

		public AdminVehicleController(IAdminVehicleService vehicleService)
		{
			_adminVehicleService = vehicleService;
		}

		[HttpPost]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> CreateVehicle([FromForm] VehicleDTO dto)
		{
			try
			{
				var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
				var vehicle = await _adminVehicleService.CreateVehicleAsync(organizationId, dto);

				return Ok(new
				{
					message = "Vehicle created successfully",
					vehicleId = vehicle.Id,
					vehicle.OrganizationId,
					vehicle.VehicleName,
					vehicle.PlatNomor,
				});
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		[HttpGet]
		public async Task<IActionResult> GetVehicles()
		{
			try
			{
				var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
				var vehicles = await _adminVehicleService.GetVehiclesByOrganizationAsync(organizationId);
				return Ok(vehicles);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while retrieving vehicles", error = ex.Message });
			}
		}

		[HttpGet("{vehicleId}")]
		public async Task<IActionResult> GetVehicleById(int vehicleId)
		{
			try
			{
				var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
				var vehicle = await _adminVehicleService.GetVehicleByIdAsync(organizationId, vehicleId);

				return Ok(vehicle);  // ✅ Sudah return GetVehicleDTO dengan efficiency
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred", error = ex.Message });
			}
		}

		[HttpPut("{vehicleId}")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> UpdateVehicle(int vehicleId, [FromForm] UpdateVehicleDTO dto)
		{
			try
			{
				var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
				var vehicle = await _adminVehicleService.UpdateVehicleAsync(organizationId, vehicleId, dto);

				return Ok(new
				{
					message = "Vehicle updated successfully",
					vehicleId = vehicle.Id,
					vehicle.DriverId,
					vehicle.VehicleName,
					vehicle.PlatNomor,
					vehicle.TahunPembuatan,
					vehicle.Odometer,
					vehicle.Tipe,
					vehicle.Status,
					vehicle.MuatanMax,
					vehicle.TipeBensin,
					vehicle.ImageUrl
				});
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while updating vehicle", error = ex.Message });
			}
		}

		[HttpDelete("{vehicleId}")]
		public async Task<IActionResult> DeleteVehicle(int vehicleId)
		{
			try
			{
				var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
				var vehicle = await _adminVehicleService.DeleteVehicleAsync(organizationId, vehicleId);

				return Ok(new
				{
					message = "Vehicle deleted successfully",
					vehicleId = vehicle.Id,
					vehicle.VehicleName
				});
			}
			catch (KeyNotFoundException ex)
			{
				return NotFound(new { message = ex.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "An error occurred while deleting vehicle", error = ex.Message });
			}
		}
	}
}
