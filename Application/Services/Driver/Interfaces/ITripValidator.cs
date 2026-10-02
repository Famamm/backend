using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface ITripValidator
    {
        void ValidateDriverForTrip(int authenticatedDriverId, int requestedDriverId);
        void ValidateNoActiveTrip(Trip? activeTrip);
        void ValidateVehicleForTrip(Vehicle vehicle, int driverId, int? organizationId);
        void ValidateStopTrip(Trip trip, Driver driver, Vehicle vehicle, StopTripDTO dto);
        void ValidateCancelTrip(Trip trip, Driver driver, int driverId);
        void ValidateOdometer(int? odometerAkhir, int? odometerAwal);
    }
}
