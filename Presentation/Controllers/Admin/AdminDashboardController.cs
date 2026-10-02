using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/dashboard")]
    [Authorize(Roles = "Admin")]
    public class DashboardAdminController : ControllerBase
    {
        private readonly IAdminDashboardService _dashboardAdminService;

        public DashboardAdminController(IAdminDashboardService dashboardAdminService)
        {
            _dashboardAdminService = dashboardAdminService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetAdminDashboard([FromQuery] int? bulan, [FromQuery] int? tahun, [FromQuery] string? rangeArmada, [FromQuery] string? rangeBbm)
        {
            try
            {
                var currentAdminId = ClaimsHelper.GetUserIdOrThrow(User);

                var dashboard = await _dashboardAdminService.GetAdminDashboardAsync(currentAdminId, bulan, tahun, rangeArmada, rangeBbm);
                return Ok(dashboard);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while loading dashboard.", error = ex.Message });
            }
        }

        [HttpGet("statistics")]
        [HttpGet("/api/admin/statistics")]
        public async Task<IActionResult> GetAdminStatistics([FromQuery] int? bulan, [FromQuery] int? tahun, [FromQuery] string? rangeArmada, [FromQuery] string? rangeBbm)
        {
            try
            {
                var currentAdminId = ClaimsHelper.GetUserIdOrThrow(User);

                var dashboard = await _dashboardAdminService.GetAdminDashboardAsync(currentAdminId, bulan, tahun, rangeArmada, rangeBbm);
                return Ok(new
                {
                    message = "Statistik admin berhasil diambil",
                    data = dashboard
                });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil statistik admin.", error = ex.Message });
            }
        }
    }
}