using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/drivers/inspections")]
    [Authorize(Roles = "Driver")]
    public class DriverInspectionsController : ControllerBase
    {
        private readonly IDriverInspectionService _inspectionService;

        public DriverInspectionsController(IDriverInspectionService inspectionService)
        {
            _inspectionService = inspectionService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateInspection([FromForm] CreateInspectionDTO dto)
        {
            try
            {
                var inspection = await _inspectionService.CreateInspectionAsync(dto);

                return Ok(new
                {
                    message = "Inspection created successfully",
                    inspection
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the inspection", error = ex.Message });
            }
        }

        [HttpGet("driver/{driverId}")]
        public async Task<IActionResult> GetInspectionsByDriver(int driverId)
        {
            try
            {
                var inspections = await _inspectionService.GetInspectionsByDriverAsync(driverId);

                return Ok(new
                {
                    message = "Inspections retrieved successfully",
                    count = inspections.Count,
                    inspections
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving inspections", error = ex.Message });
            }
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyInspections()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "User ID tidak ditemukan dalam token" });
                }

                var inspections = await _inspectionService.GetInspectionsByUserIdAsync(userId);

                return Ok(new
                {
                    message = "Inspections retrieved successfully",
                    count = inspections.Count,
                    inspections
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving inspections", error = ex.Message });
            }
        }

        [HttpGet("me/today")]
        public async Task<IActionResult> GetMyTodayInspections()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "User ID tidak ditemukan dalam token" });
                }

                var inspections = await _inspectionService.GetTodayInspectionsByUserIdAsync(userId);

                return Ok(new
                {
                    message = "Inspections retrieved successfully",
                    count = inspections.Count,
                    inspections
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving inspections", error = ex.Message });
            }
        }

        [HttpGet("vehicle/{vehicleId}")]
        public async Task<IActionResult> GetInspectionsByVehicle(int vehicleId)
        {
            try
            {
                var inspections = await _inspectionService.GetInspectionsByVehicleAsync(vehicleId);

                return Ok(new
                {
                    message = "Inspections retrieved successfully",
                    count = inspections.Count,
                    inspections
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving inspections", error = ex.Message });
            }
        }

        [HttpGet("{inspectionId}")]
        public async Task<IActionResult> GetInspectionDetail(int inspectionId)
        {
            try
            {
                var inspection = await _inspectionService.GetInspectionDetailByIdAsync(inspectionId);

                if (inspection == null)
                {
                    return NotFound(new { message = "Inspection detail not found" });
                }

                return Ok(inspection);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving the inspection detail", error = ex.Message });
            }
        }

        [HttpPut("{inspectionId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateInspection(int inspectionId, [FromForm] UpdateInspectionDTO dto)
        {
            try
            {
                var inspection = await _inspectionService.UpdateInspectionAsync(inspectionId, dto);

                return Ok(new
                {
                    message = "Inspection updated successfully",
                    inspection
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
                return StatusCode(500, new { message = "An error occurred while updating the inspection", error = ex.Message });
            }
        }

        [HttpDelete("{inspectionId}")]
        public async Task<IActionResult> DeleteInspection(int inspectionId)
        {
            try
            {
                var result = await _inspectionService.DeleteInspectionAsync(inspectionId);

                if (!result)
                {
                    return NotFound(new { message = "Inspection not found" });
                }

                return Ok(new { message = "Inspection deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the inspection", error = ex.Message });
            }
        }

        [HttpGet("inspection-items")]
        public async Task<IActionResult> GetInspectionItems()
        {
            try
            {
                var items = await _inspectionService.GetInspectionItemsAsync();

                return Ok(new
                {
                    message = "Inspection items retrieved successfully",
                    count = items.Count,
                    items
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving inspection items", error = ex.Message });
            }
        }
    }
}
