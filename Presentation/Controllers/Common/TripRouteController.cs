using fleetops_backend.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fleetops_backend.Presentation.Controllers
{
    [ApiController]
    [Route("api/trips")]
    [Authorize(Roles = "Admin,FleetManager")]
    public class TripRouteController : ControllerBase
    {
        private readonly ITripRouteService _tripRouteService;

        public TripRouteController(ITripRouteService tripRouteService)
        {
            _tripRouteService = tripRouteService;
        }

        [HttpGet("live-locations")]
        public async Task<IActionResult> GetLiveLocations()
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var locations = await _tripRouteService.GetLiveLocationsAsync(organizationId);

                return Ok(new
                {
                    message = "Posisi driver yang sedang bertugas berhasil diambil",
                    count = locations.Count,
                    data = locations
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil posisi driver", error = ex.Message });
            }
        }

        [HttpGet("{tripId}/gps")]
        public async Task<IActionResult> GetTripRoute(int tripId)
        {
            try
            {
                var organizationId = ClaimsHelper.GetOrganizationIdOrThrow(User);
                var route = await _tripRouteService.GetTripRouteAsync(organizationId, tripId);

                return Ok(new
                {
                    message = "Rute perjalanan berhasil diambil",
                    data = route
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
                return StatusCode(500, new { message = "Terjadi kesalahan saat mengambil rute perjalanan", error = ex.Message });
            }
        }
    }
}
