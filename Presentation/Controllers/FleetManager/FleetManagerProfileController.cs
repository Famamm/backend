using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/fleet-manager/profile")]
    [Authorize(Roles = "FleetManager")]
    public class FleetManagerProfileController : ControllerBase
    {
        private readonly IFleetManagerProfileService _profileService;

        public FleetManagerProfileController(IFleetManagerProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            try
            {
                var userId = ClaimsHelper.GetUserIdOrThrow(User);
                var profile = await _profileService.GetProfileAsync(userId);

                return Ok(new
                {
                    message = "Profil fleet manager berhasil diambil",
                    data = profile
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil profil fleet manager", error = ex.Message });
            }
        }

        [HttpPut("me")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateMyProfile([FromForm] FleetManagerProfileDTO dto)
        {
            try
            {
                var userId = ClaimsHelper.GetUserIdOrThrow(User);
                var profile = await _profileService.UpdateProfileAsync(userId, dto);

                return Ok(new
                {
                    message = "Profil Anda berhasil diperbarui",
                    data = profile
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat memperbarui profil fleet manager", error = ex.Message });
            }
        }
    }
}
