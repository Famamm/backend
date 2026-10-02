using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface ITripService
    {
        Task<Trip?> StartTripAsync(int userId, StartTripDTO dto);
        Task<Trip> StopTripAsync(int tripId, int userId, StopTripDTO dto);
        Task<Trip> CancelTripAsync(int tripId, int userId);
        Task<List<GetTripDTO>> GetTripsByDriverAsync(int driverId);
        Task<GetDetailTripDTO?> GetDetailTripAsync(int tripId);
        Task<GetActiveTripDTO?> GetActiveTripAsync(int driverId);
    }
}
