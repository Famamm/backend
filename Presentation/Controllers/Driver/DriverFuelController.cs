using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.Services;
using fleetops_backend.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace fleetops_backend.Controllers
{
    [ApiController]
    [Route("api/drivers/fuel")]
    [Authorize(Roles = "Driver")]
    public class DriverFuelController : ControllerBase
    {
        private readonly IDriverFuelService _fuelService;
        private readonly AppDbContext _context;

        public DriverFuelController(IDriverFuelService fuelService, AppDbContext context)
        {
            _fuelService = fuelService;
            _context = context;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RecordFuel([FromForm] CreateFuelDTO dto)
        {
            try
            {
                var userId = ClaimsHelper.GetUserIdOrThrow(User);
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var result = await _fuelService.LogFuelAsync(userId, organizationId, dto);
                return Ok(new { message = "Pencatatan pengisian BBM berhasil disimpan!", data = result });
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
                return StatusCode(500, new { message = "Gagal menyimpan pencatatan BBM.", error = ex.Message });
            }
        }

        [HttpGet("prices")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFuelPrices()
        {
            try
            {
                var prices = await _context.FuelPrices
                    .OrderBy(f => f.Id)
                    .Select(f => new { f.Id, f.FuelType, f.PricePerLiter })
                    .ToListAsync();
                return Ok(prices);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Gagal mengambil daftar harga BBM.", error = ex.Message });
            }
        }

        [HttpGet("efficiency")]
        public async Task<IActionResult> GetFuelEfficiency()
        {
            try
            {
                var data = await _fuelService.GetVehicleEfficiencyListAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Gagal melakukan kalkulasi efisiensi BBM.", error = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetFuelHistory()
        {
            try
            {
                var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);

                if (userClaim == null || !int.TryParse(userClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Driver ID tidak valid atau tidak ditemukan dalam token." });
                }

                var result = await _fuelService.GetFuelHistoryByUserIdAsync(userId);

                return Ok(new
                {
                    message = "Berhasil mengambil riwayat pengisian BBM.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan internal server.", error = ex.Message });
            }
        }

        [HttpGet("{fuelId}")]
        public async Task<IActionResult> GetFuelDetailById(int fuelId)
        {
            try
            {
                var result = await _fuelService.GetFuelDetailByIdAsync(fuelId);

                if (result == null)
                {
                    return NotFound(new { message = "Detail pengisian BBM tidak ditemukan." });
                }

                return Ok(new
                {
                    message = "Berhasil mengambil detail pengisian BBM.",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan internal server.", error = ex.Message });
            }
        }
    }
}