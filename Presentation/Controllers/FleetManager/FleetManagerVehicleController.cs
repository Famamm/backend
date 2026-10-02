using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/fleet-manager")]
    [Authorize(Roles = "FleetManager")]
    public class FleetManagerVehicleController : ControllerBase
    {
        private readonly IFleetManagerVehicleService _vehicleService;

        public FleetManagerVehicleController(IFleetManagerVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet("summary")]
        public async Task<IActionResult> GetSummary()
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var summary = await _vehicleService.GetSummaryAsync(organizationId);

                return Ok(new
                {
                    message = "Ringkasan armada berhasil diambil",
                    data = summary
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil ringkasan armada", error = ex.Message });
            }
        }

        [HttpGet("vehicles")]
        public async Task<IActionResult> GetVehicles()
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var vehicles = await _vehicleService.GetVehiclesAsync(organizationId);

                return Ok(new
                {
                    message = "Daftar kendaraan berhasil diambil",
                    count = vehicles.Count,
                    vehicles
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil daftar kendaraan", error = ex.Message });
            }
        }
    }
}
