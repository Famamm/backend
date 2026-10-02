using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/fleet-manager")]
    [Authorize(Roles = "FleetManager")]
    public class FleetManagerStatisticsController : ControllerBase
    {
        private readonly IFleetManagerStatisticsService _statisticsService;

        public FleetManagerStatisticsController(IFleetManagerStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics([FromQuery] int? bulan, [FromQuery] int? tahun)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var statistics = await _statisticsService.GetStatisticsAsync(organizationId, bulan, tahun);

                return Ok(new
                {
                    message = "Statistik armada berhasil diambil",
                    data = statistics
                });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil statistik armada", error = ex.Message });
            }
        }
    }
}
