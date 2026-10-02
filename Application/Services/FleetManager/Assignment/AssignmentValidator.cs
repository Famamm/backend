using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class AssignmentValidator : IAssignmentValidator
    {
        public void ValidateVehicleExists(Vehicle? vehicle)
        {
            if (vehicle == null)
                throw new KeyNotFoundException("Kendaraan tidak ditemukan.");
        }

        public void ValidateDriverExists(Driver? driver)
        {
            if (driver == null)
                throw new KeyNotFoundException("Driver tidak ditemukan.");
        }

        public void ValidateDriverIsActive(Driver driver)
        {
            if (driver.Status == "Deleted")
                throw new InvalidOperationException("Driver sudah dinonaktifkan.");
        }

        public void ValidateVehicleIsActive(Vehicle vehicle)
        {
            if (vehicle.Status != "Active")
                throw new InvalidOperationException("Kendaraan tidak berstatus Active.");
        }

        public void ValidateDriverNotOnTrip(bool hasOngoingTrip, string namaDriver)
        {
            if (hasOngoingTrip)
            {
                throw new InvalidOperationException(
                    $"{namaDriver} sedang dalam perjalanan. Tunggu trip selesai sebelum mengubah penugasan kendaraan.");
            }
        }
    }
}
