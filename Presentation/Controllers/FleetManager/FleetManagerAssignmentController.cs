using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/fleet-manager")]
    [Authorize(Roles = "FleetManager")]
    public class FleetManagerAssignmentController : ControllerBase
    {
        private readonly IFleetManagerAssignmentService _assignmentService;

        public FleetManagerAssignmentController(IFleetManagerAssignmentService assignmentService)
        {
            _assignmentService = assignmentService;
        }

        [HttpPost("assignments")]
        public async Task<IActionResult> AssignDriver([FromBody] AssignVehicleDTO dto)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var result = await _assignmentService.AssignDriverAsync(organizationId, dto);

                return Ok(new
                {
                    message = "Driver berhasil ditugaskan ke kendaraan",
                    data = result
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat menugaskan driver", error = ex.Message });
            }
        }

        [HttpDelete("vehicles/{vehicleId}/driver")]
        public async Task<IActionResult> UnassignDriver(int vehicleId)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var result = await _assignmentService.UnassignDriverAsync(organizationId, vehicleId);

                return Ok(new
                {
                    message = "Driver berhasil dilepas dari kendaraan",
                    data = result
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat melepas driver", error = ex.Message });
            }
        }

        [HttpGet("vehicles/{vehicleId}/current-driver")]
        public async Task<IActionResult> GetCurrentDriver(int vehicleId)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var result = await _assignmentService.GetCurrentDriverAsync(organizationId, vehicleId);

                return Ok(new
                {
                    message = "Data driver kendaraan berhasil diambil",
                    data = result
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil data driver", error = ex.Message });
            }
        }

        [HttpGet("drivers/available")]
        public async Task<IActionResult> GetAvailableDrivers()
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var drivers = await _assignmentService.GetAvailableDriversAsync(organizationId);

                return Ok(new
                {
                    message = "Daftar driver tersedia berhasil diambil",
                    count = drivers.Count,
                    drivers
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil daftar driver", error = ex.Message });
            }
        }
    }
}
