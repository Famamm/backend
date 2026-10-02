using fleetops_backend.Application.DTOs;

namespace fleetops_backend.Application.Services
{
    public interface ITripGpsService
    {
        Task<SaveTripGpsResultDTO> SaveGpsPointsAsync(
            int tripId,
            int userId,
            SaveTripGpsDTO dto);
    }
}