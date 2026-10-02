using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/drivers/vehicles/me")]
    [Authorize(Roles = "Driver")]
    public class DriverMaintenanceController : ControllerBase
    {
        private readonly IDriverMaintenanceService _maintenanceService;

        public DriverMaintenanceController(IDriverMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        [HttpGet("maintenance-status")]
        public async Task<IActionResult> GetMaintenanceStatus()
        {
            try
            {
                var userId = ClaimsHelper.GetUserIdOrThrow(User);
                var status = await _maintenanceService.GetMaintenanceStatusAsync(userId);

                return Ok(new
                {
                    message = "Status pemeliharaan kendaraan berhasil diambil",
                    data = status
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil status pemeliharaan", error = ex.Message });
            }
        }
    }
}
