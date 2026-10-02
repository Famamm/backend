using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface ITripRouteService
    {
        Task<TripRouteDTO> GetTripRouteAsync(int organizationId, int tripId);
        Task<List<DriverLiveLocationDTO>> GetLiveLocationsAsync(int organizationId);
    }
}
