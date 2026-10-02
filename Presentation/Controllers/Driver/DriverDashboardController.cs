using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/drivers/dashboard")]
    [Authorize(Roles = "Driver")]
    public class DriverDashboardController : ControllerBase
    {
        private readonly IDriverDashboardService _dashboardService;

        public DriverDashboardController(IDriverDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetDashboardDriver()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "User ID tidak ditemukan dalam token" });
                }

                var dashboard = await _dashboardService.GetDashboardDriverByUserIdAsync(userId);
                return Ok(dashboard);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while loading dashboard." });
            }
        }
    }
}