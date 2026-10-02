using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/fleet-managers")]
    [Authorize(Roles = "Admin")]
    public class AdminFleetManagerController : ControllerBase
    {
        private readonly IAdminFleetManagerService _fleetManagerService;

        public AdminFleetManagerController(IAdminFleetManagerService fleetManagerService)
        {
            _fleetManagerService = fleetManagerService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateFleetManager([FromForm] CreateFleetManagerDTO dto)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var (user, generatedPassword, username) = await _fleetManagerService.CreateFleetManagerAsync(organizationId, dto);

                return Ok(new
                {
                    message = "Fleet manager created successfully",
                    userId = user.Id,
                    user.OrganizationId,
                    fullName = user.FullName,
                    address = user.Address,
                    phoneNumber = user.PhoneNumber,
                    profilePhoto = user.ProfilePhoto,
                    username,
                    generatedPassword
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
                return StatusCode(500, new { message = "An error occurred while creating fleet manager", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFleetManagers()
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var fleetManagers = await _fleetManagerService.GetFleetManagersAsync(organizationId);

                return Ok(new
                {
                    message = "Fleet managers retrieved successfully",
                    count = fleetManagers.Count,
                    fleetManagers
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving fleet managers", error = ex.Message });
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFleetManagerById(int userId)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var fleetManager = await _fleetManagerService.GetFleetManagerByIdAsync(organizationId, userId);

                return Ok(fleetManager);
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
                return StatusCode(500, new { message = "An error occurred while retrieving fleet manager", error = ex.Message });
            }
        }

        [HttpPut("{userId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateFleetManager(int userId, [FromForm] UpdateFleetManagerDTO dto)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var user = await _fleetManagerService.UpdateFleetManagerAsync(organizationId, userId, dto);

                return Ok(new { message = "Fleet manager updated successfully", userId = user.Id });
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
                return StatusCode(500, new { message = "An error occurred while updating fleet manager", error = ex.Message });
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteFleetManager(int userId)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                await _fleetManagerService.DeleteFleetManagerAsync(organizationId, userId);

                return Ok(new { message = "Fleet manager deleted successfully" });
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
                return StatusCode(500, new { message = "An error occurred while deleting fleet manager", error = ex.Message });
            }
        }

        [HttpPost("{userId}/password/reset")]
        public async Task<IActionResult> ResetPassword(int userId)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var result = await _fleetManagerService.ResetFleetManagerPasswordAsync(organizationId, userId);

                return Ok(new
                {
                    message = "Password reset successfully",
                    generatedPassword = result.generatedPassword
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
                return StatusCode(500, new { message = "An error occurred while resetting fleet manager password", error = ex.Message });
            }
        }
    }
}
