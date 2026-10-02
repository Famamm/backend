using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class FuelValidator : IFuelValidator
    {
        public void ValidateVehicleExists(Vehicle? vehicle)
        {
            if (vehicle == null)
                throw new KeyNotFoundException("Kendaraan tidak ditemukan");
        }

        public void ValidateDriverExists(Driver? driver)
        {
            if (driver == null)
                throw new KeyNotFoundException("Driver tidak ditemukan");
        }

        public void ValidateVehicleAssignedToDriver(Vehicle vehicle, int driverId)
        {
            if (!vehicle.DriverId.HasValue)
                throw new InvalidOperationException("Kendaraan belum memiliki driver");

            if (vehicle.DriverId.Value != driverId)
                throw new InvalidOperationException("Kendaraan ini tidak ditugaskan kepada Anda");
        }

        public void ValidateFuelPriceExists(FuelPrice? fuelPrice, string fuelType)
        {
            if (fuelPrice == null)
                throw new KeyNotFoundException($"Jenis BBM '{fuelType}' tidak ditemukan. " +
                    "Gunakan salah satu: Pertalite, BioSolar, Pertamax (RON 92), Pertamax Green 95, " +
                    "Pertamax Turbo (RON 98), Dexlite, atau Pertamina Dex");
        }

        public void ValidateOdometer(int? odometerAkhir, int? odometerAwal)
        {
            if (!odometerAkhir.HasValue)
                throw new InvalidOperationException("Odometer akhir wajib diisi.");

            if (!odometerAwal.HasValue)
                throw new InvalidOperationException("Odometer awal tidak tersedia.");

            if (odometerAkhir.Value < odometerAwal.Value)
            {
                throw new InvalidOperationException("Odometer akhir tidak boleh lebih kecil dari odometer awal.");
            }
        }

        public void ValidateLocation(double? latitude, double? longitude)
        {
            // Lokasi bersifat opsional: kalau driver menolak izin GPS, pengisian tetap boleh dicatat.
            if (!latitude.HasValue && !longitude.HasValue)
            {
                return;
            }

            if (!latitude.HasValue || !longitude.HasValue)
            {
                throw new InvalidOperationException(
                    "Data lokasi tidak lengkap. Latitude dan Longitude harus dikirim bersamaan.");
            }

            if (latitude.Value < -90 || latitude.Value > 90)
            {
                throw new InvalidOperationException("Latitude harus berada di antara -90 dan 90.");
            }

            if (longitude.Value < -180 || longitude.Value > 180)
            {
                throw new InvalidOperationException("Longitude harus berada di antara -180 dan 180.");
            }
        }
    }
}
