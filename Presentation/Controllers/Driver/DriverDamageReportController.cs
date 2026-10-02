using System.Security.Claims;
using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/drivers/trips")]
    [Authorize(Roles = "Driver")]
    public class DriverDamageReportController : ControllerBase
    {
        private readonly IDriverDamageReportService _damageReportService;

        public DriverDamageReportController(IDriverDamageReportService damageReportService)
        {
            _damageReportService = damageReportService;
        }

        [HttpPost("{tripId}/damage-reports")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(int tripId, [FromForm] CreateDamageReportDTO dto)
        {
            try
            {
                var userId = ClaimsHelper.GetUserIdOrThrow(User);
                var report = await _damageReportService.CreateAsync(userId, tripId, dto);

                return StatusCode(201, new
                {
                    message = "Laporan kerusakan berhasil dikirim",
                    data = report
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengirim laporan kerusakan", error = ex.Message });
            }
        }

        [HttpGet("{tripId}/damage-reports")]
        public async Task<IActionResult> GetByTrip(int tripId)
        {
            try
            {
                var userId = ClaimsHelper.GetUserIdOrThrow(User);
                var reports = await _damageReportService.GetByTripAsync(userId, tripId);

                return Ok(new
                {
                    message = "Daftar laporan kerusakan berhasil diambil",
                    count = reports.Count,
                    data = reports
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil laporan kerusakan", error = ex.Message });
            }
        }
    }
}
