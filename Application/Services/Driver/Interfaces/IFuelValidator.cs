using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IFuelValidator
    {
        void ValidateVehicleExists(Vehicle? vehicle);
        void ValidateDriverExists(Driver? driver);
        void ValidateVehicleAssignedToDriver(Vehicle vehicle, int driverId);
        void ValidateFuelPriceExists(FuelPrice? fuelPrice, string fuelType);
        void ValidateOdometer(int? odometerAkhir, int? odometerAwal);
        void ValidateLocation(double? latitude, double? longitude);
    }
}

