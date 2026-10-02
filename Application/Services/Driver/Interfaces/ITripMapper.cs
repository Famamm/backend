using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface ITripMapper
    {
        GetTripDTO ToTripDto(Trip trip, TripDetail? detail);
        GetDetailTripDTO ToDetailTripDto(Trip trip, Driver driver, Vehicle vehicle);
        GetActiveTripDTO ToActiveTripDto(Trip trip, DateTimeOffset currentTime);
    }
}
