using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/drivers/statistics")]
    [Authorize(Roles = "Driver")]
    public class DriverStatisticsController : ControllerBase
    {
        private readonly IDriverStatisticsService _statisticsService;

        public DriverStatisticsController(IDriverStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyStatistics()
        {
            try
            {
                var userId = ClaimsHelper.GetUserIdOrThrow(User);
                var statistics = await _statisticsService.GetStatisticsByUserIdAsync(userId);

                return Ok(new
                {
                    message = "Statistik driver berhasil diambil",
                    data = statistics
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil statistik driver", error = ex.Message });
            }
        }
    }
}
