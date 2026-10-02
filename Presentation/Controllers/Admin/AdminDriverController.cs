using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/drivers")]
    [Authorize(Roles = "Admin")]
    public class AdminDriverController : ControllerBase
    {
        private readonly IAdminDriverService _adminDriverService;

        public AdminDriverController(IAdminDriverService adminDriverService)
        {
            _adminDriverService = adminDriverService;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateDriver([FromForm] CreateDriverDTO dto)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var (driver, generatedPassword, username) = await _adminDriverService.CreateDriverAsync(organizationId, dto);

                var offset = TimeSpan.FromHours(7);
                return Ok(new
                {
                    message = "Driver created successfully",
                    driverId = driver.Id,
                    userId = driver.UserId,
                    driver.OrganizationId,
                    address = driver.User?.Address,
                    profilePhoto = driver.User?.ProfilePhoto,
                    fotoLisensi = driver.FotoLisensi,
                    driver.Status,
                    driver.JenisLisensi,
                    TanggalBerlaku = driver.TanggalBerlaku?.ToOffset(offset),
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
                return StatusCode(500, new { message = "An error occurred while creating driver", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDrivers()
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var drivers = await _adminDriverService.GetDriversAsync(organizationId);

                var offset = TimeSpan.FromHours(7);
                var result = drivers.Select(d => new
                {
                    d.Id,
                    d.UserId,
                    d.User?.FullName,
                    d.User?.Username,
                    d.User?.Email,
                    d.User?.Address,
                    d.User?.ProfilePhoto,
                    d.JenisLisensi,
                    d.FotoLisensi,
                    TanggalBerlaku = d.TanggalBerlaku?.ToOffset(offset),
                    d.Status
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching drivers", error = ex.Message });
            }
        }

        [HttpGet("{driverId}")]
        public async Task<IActionResult> GetDriverById(int driverId)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var driver = await _adminDriverService.GetDriverByIdAsync(organizationId, driverId);

                var offset = TimeSpan.FromHours(7);
                return Ok(driver);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching driver", error = ex.Message });
            }
        }

        [HttpPut("{driverId}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateDriver(int driverId, [FromForm] UpdateDriverDTO dto)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var driver = await _adminDriverService.UpdateDriverAsync(organizationId, driverId, dto);

                return Ok(new { message = "Driver updated successfully", driverId = driver.Id });
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
                return StatusCode(500, new { message = "An error occurred while updating driver", error = ex.Message });
            }
        }

        [HttpDelete("{driverId}")]
        public async Task<IActionResult> DeleteDriver(int driverId)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                await _adminDriverService.DeleteDriverAsync(organizationId, driverId);
                return Ok(new { message = "Driver deleted successfully" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting driver", error = ex.Message });
            }
        }        

        [HttpPost("{driverId}/password/reset")]
        public async Task<IActionResult> ResetPassword(int driverId)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var result = await _adminDriverService.ResetDriverPasswordAsync(organizationId, driverId);

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
                return StatusCode(500, new { message = "An error occurred while resetting driver password", error = ex.Message });
            }
        }
    }
}
