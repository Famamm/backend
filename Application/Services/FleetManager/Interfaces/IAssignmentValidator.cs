using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IAssignmentValidator
    {
        void ValidateVehicleExists(Vehicle? vehicle);
        void ValidateDriverExists(Driver? driver);
        void ValidateDriverIsActive(Driver driver);
        void ValidateVehicleIsActive(Vehicle vehicle);
        void ValidateDriverNotOnTrip(bool hasOngoingTrip, string namaDriver);
    }
}
