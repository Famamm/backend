using fleetops_backend.Application.Services;
using fleetops_backend.Application.DTOs.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/admin/profile")]
    [Authorize(Roles = "Admin")]
    public class ProfileAdminController : ControllerBase
    {
        private readonly IAdminProfileService _adminProfileService;

        public ProfileAdminController(IAdminProfileService adminProfileService)
        {
            _adminProfileService = adminProfileService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (adminIdClaim == null || !int.TryParse(adminIdClaim.Value, out int adminId))
                {
                    return Unauthorized(new { message = "Admin ID tidak ditemukan dalam token" });
                }

                var profile = await _adminProfileService.GetAdminProfileAsync(adminId);

                return Ok(new
                {
                    message = "Profil admin berhasil diambil",
                    data = profile
                });
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

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMyProfile([FromForm] UpdateAdminProfileDTO dto)
        {
            try
            {
                var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (adminIdClaim == null || !int.TryParse(adminIdClaim.Value, out int adminId))
                {
                    return Unauthorized(new { message = "Admin ID tidak ditemukan dalam token" });
                }

                var updatedProfile = await _adminProfileService.UpdateAdminProfileAsync(adminId, dto);

                return Ok(new
                {
                    message = "Profil Anda berhasil diperbarui",
                    data = updatedProfile
                });
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
