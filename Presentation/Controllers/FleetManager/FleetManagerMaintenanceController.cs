using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/fleet-manager")]
    [Authorize(Roles = "FleetManager")]
    public class FleetManagerMaintenanceController : ControllerBase
    {
        private readonly IFleetManagerMaintenanceService _maintenanceService;

        public FleetManagerMaintenanceController(IFleetManagerMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        [HttpPost("vehicles/{vehicleId}/maintenances")]
        public async Task<IActionResult> Create(int vehicleId, [FromBody] MaintenanceScheduleDTO dto)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var maintenance = await _maintenanceService.CreateAsync(organizationId, vehicleId, dto);

                return StatusCode(201, new
                {
                    message = "Jadwal pemeliharaan berhasil dibuat",
                    data = maintenance
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat membuat jadwal pemeliharaan", error = ex.Message });
            }
        }

        [HttpGet("vehicles/{vehicleId}/maintenances")]
        public async Task<IActionResult> GetByVehicle(int vehicleId)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var maintenances = await _maintenanceService.GetByVehicleAsync(organizationId, vehicleId);

                return Ok(new
                {
                    message = "Daftar pemeliharaan kendaraan berhasil diambil",
                    count = maintenances.Count,
                    data = maintenances
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil daftar pemeliharaan", error = ex.Message });
            }
        }

        [HttpGet("vehicles/maintenance-status")]
        public async Task<IActionResult> GetFleetStatus()
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var status = await _maintenanceService.GetFleetStatusAsync(organizationId);

                return Ok(new
                {
                    message = "Status pemeliharaan armada berhasil diambil",
                    count = status.Count,
                    data = status
                });
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

        [HttpGet("maintenances/{maintenanceId}")]
        public async Task<IActionResult> GetById(int maintenanceId)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var maintenance = await _maintenanceService.GetByIdAsync(organizationId, maintenanceId);

                return Ok(new
                {
                    message = "Detail pemeliharaan berhasil diambil",
                    data = maintenance
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil detail pemeliharaan", error = ex.Message });
            }
        }

        [HttpPut("maintenances/{maintenanceId}")]
        public async Task<IActionResult> Update(int maintenanceId, [FromBody] MaintenanceScheduleDTO dto)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var maintenance = await _maintenanceService.UpdateAsync(organizationId, maintenanceId, dto);

                return Ok(new
                {
                    message = "Jadwal pemeliharaan berhasil diperbarui",
                    data = maintenance
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat memperbarui jadwal pemeliharaan", error = ex.Message });
            }
        }

        [HttpDelete("maintenances/{maintenanceId}")]
        public async Task<IActionResult> Delete(int maintenanceId)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                await _maintenanceService.DeleteAsync(organizationId, maintenanceId);

                return Ok(new { message = "Jadwal pemeliharaan berhasil dihapus" });
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat menghapus jadwal pemeliharaan", error = ex.Message });
            }
        }

        [HttpPut("maintenances/{maintenanceId}/status")]
        public async Task<IActionResult> UpdateStatus(int maintenanceId, [FromBody] UpdateMaintenanceStatusDTO dto)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var maintenance = await _maintenanceService.UpdateStatusAsync(organizationId, maintenanceId, dto);

                return Ok(new
                {
                    message = "Status pemeliharaan berhasil diperbarui",
                    data = maintenance
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat memperbarui status pemeliharaan", error = ex.Message });
            }
        }
    }
}
