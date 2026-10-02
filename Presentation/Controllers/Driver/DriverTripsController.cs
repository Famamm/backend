using System.Security.Claims;
using fleetops_backend.Application.DTOs;
using fleetops_backend.Application.Services;
using fleetops_backend.Common;
using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/drivers/trips")]
    [Authorize(Roles = "Driver")]
    public class DriverTripsController : ControllerBase
    {
        private readonly ITripService _tripService;
        private readonly ITripGpsService _tripGpsService;

        public DriverTripsController(
            ITripService tripService,
            ITripGpsService tripGpsService)
        {
            _tripService = tripService;
            _tripGpsService = tripGpsService;
        }

        [HttpPost]
        public async Task<IActionResult> StartTrip(StartTripDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "User ID tidak ditemukan dalam token" });
                }
                
                var trip = await _tripService.StartTripAsync(userId, dto);

                if (trip == null)
                {
                    return BadRequest(new { message = "Invalid driver or vehicle." });
                }

                return Ok(new
                {
                    message = "Trip started successfully",
                    trip.Id,
                    trip.DriverId,
                    trip.VehicleId,
                    trip.LokasiAwal,
                    trip.LokasiAkhir,
                    trip.OdometerAwal,
                    trip.WaktuMulai,
                    trip.Status
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while starting the trip." });
            }
        }

        [HttpPatch("{tripId}/stop")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> StopTrip(int tripId, [FromForm] StopTripDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "User ID tidak ditemukan dalam token" });
                }

                if (dto.Image == null || dto.Image.Length == 0)
                {
                    return BadRequest(new { message = "Image is required." });
                }

                var trip = await _tripService.StopTripAsync(tripId, userId, dto);

                var tripDetail = trip.Detail.FirstOrDefault();

                return Ok(new
                {
                    message = "Trip stopped successfully",
                    trip.Id,
                    trip.LokasiAkhir,
                    trip.OdometerAkhir,
                    jarak = tripDetail?.Jarak ?? 0,
                    avgSpeed = tripDetail?.AvgSpeed ?? 0,
                    waktuJam = tripDetail?.Waktu > 0
                        ? (double)tripDetail.Waktu / 3600.0
                        : 0,
                    imageUrl = tripDetail?.ImageUrl,
                    trip.WaktuSelesai,
                    trip.Status
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
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Database error." });
            }
        }

        [HttpPost("{tripId}/gps")]
        [EnableRateLimiting("gps-batch")]
        public async Task<IActionResult> SaveGpsPoints(
            int tripId,
            [FromBody] SaveTripGpsDTO dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "User ID tidak ditemukan dalam token" });
                }

                var result = await _tripGpsService.SaveGpsPointsAsync(
                    tripId,
                    userId,
                    dto);

                return Ok(new
                {
                    message = "Batch titik GPS berhasil diproses",
                    tripId,
                    receivedCount = result.ReceivedCount,
                    savedCount = result.SavedCount,
                    duplicateCount = result.DuplicateCount
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
            catch (DbUpdateException)
            {
                return StatusCode(500, new
                {
                    message = "Gagal menyimpan titik GPS."
                });
            }
        }

        [HttpPatch("{tripId}/cancel")]
        public async Task<IActionResult> CancelTrip(int tripId, [FromBody] CancelTripDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { message = "Request body is required." });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "User ID tidak ditemukan dalam token" });
                }

                var trip = await _tripService.CancelTripAsync(tripId, userId);

                return Ok(new
                {
                    message = "Trip canceled successfully",
                    trip.Id,
                    trip.DriverId,
                    trip.VehicleId,
                    trip.Status,
                    trip.WaktuSelesai
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
            catch (Exception)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while canceling the trip." });
            }
        }

        [HttpGet("driver/{driverId}")]
        public async Task<IActionResult> GetTripByDriverId(int driverId)
        {
            var trips = await _tripService.GetTripsByDriverAsync(driverId);

            if (trips.Count == 0)
            {
                return NotFound(new { message = "No trips found for the specified driver." });
            }

            return Ok(trips);
        }

        [HttpGet("{tripId}")]
        public async Task<IActionResult> GetDetailTrip(int tripId)
        {
            var trip = await _tripService.GetDetailTripAsync(tripId);

            if (trip == null)
            {
                return NotFound(new { message = "Trip not found." });
            }

            return Ok(trip);
        }

        [HttpGet("driver/{driverId}/active")]
        public async Task<IActionResult> GetActiveTrip(int driverId)
        {
            try
            {
                var activeTrip = await _tripService.GetActiveTripAsync(driverId);

                if (activeTrip == null)
                    return NoContent();

                return Ok(new ApiResponse<GetActiveTripDTO>
                {
                    Success = true,
                    Message = "Trip aktif ditemukan",
                    Data = activeTrip
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Terjadi kesalahan pada server"
                });
            }
        }
    }
}