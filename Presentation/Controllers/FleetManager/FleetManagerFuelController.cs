using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/fleet-manager")]
    [Authorize(Roles = "FleetManager")]
    public class FleetManagerFuelController : ControllerBase
    {
        private readonly IFleetManagerFuelService _fuelService;

        public FleetManagerFuelController(IFleetManagerFuelService fuelService)
        {
            _fuelService = fuelService;
        }

        [HttpGet("vehicles/{vehicleId}/fuel-baseline")]
        public async Task<IActionResult> GetFuelBaseline(int vehicleId)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var baseline = await _fuelService.GetVehicleBaselineAsync(organizationId, vehicleId);

                return Ok(new
                {
                    message = "Baseline konsumsi BBM berhasil diambil",
                    data = baseline
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
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil baseline BBM", error = ex.Message });
            }
        }
    }
}
