using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            try
            {
                var response = await _authService.LoginAsync(dto.Identity, dto.Password);

                if (response == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterOrganizationDTO dto)
        {
            try
            {
                var response = await _authService.RegisterAsync(dto.OrganizationName, dto.FullName, dto.Email, dto.Password);

                if (response == null)
                    return BadRequest(new { message = "Email already registered" });

                return Ok(new { message = "Registration successful", data = response });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDTO dto)
        {
            try
            {
                var response = await _authService.RefreshAccessTokenAsync(dto.RefreshToken);

                if (response == null)
                    return Unauthorized(new { message = "Invalid or expired refresh token" });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while refreshing token", error = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDTO dto)
        {
            try
            {
                var result = await _authService.LogoutAsync(dto.RefreshToken);

                if (!result)
                    return BadRequest(new { message = "Refresh token not found" });

                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during logout", error = ex.Message });
            }
        }

        [HttpPost("logout-all")]
        public async Task<IActionResult> LogoutAllDevices()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                    return Unauthorized(new { message = "Invalid user" });

                var result = await _authService.LogoutAllDevicesAsync(userId);

                if (!result)
                    return BadRequest(new { message = "Failed to logout from all devices" });

                return Ok(new { message = "Logged out from all devices successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }
    }
}