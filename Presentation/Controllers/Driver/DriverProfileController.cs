using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/drivers/profile")]
    [Authorize(Roles = "Driver")]
    public class DriverProfileController : ControllerBase
    {
        private readonly IDriverProfileService _driverProfileService;

        public DriverProfileController(IDriverProfileService driverProfileService)
        {
            _driverProfileService = driverProfileService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "User ID tidak ditemukan dalam token" });
                }

                var profile = await _driverProfileService.GetDriverProfileByUserIdAsync(userId);
                return Ok(new { message = "Profil Anda berhasil diambil", data = profile });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
