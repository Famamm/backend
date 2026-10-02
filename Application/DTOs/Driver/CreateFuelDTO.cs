using Microsoft.AspNetCore.Http;

namespace fleetops_backend.Application.DTOs
{
    public class CreateFuelDTO
    {
        public int VehicleId { get; set; }
        public string FuelType { get; set; } = string.Empty;
        public decimal JmlLiter { get; set; }
        public int HargaTotal { get; set; } = 0;
        public int OdometerIsi { get; set; } = 0;
        public string? Catatan { get; set; } = string.Empty;
        public IFormFile? FuelPhoto { get; set; }

        // Lokasi pengisian (otomatis dari GPS perangkat saat driver mengisi BBM)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? AccuracyMeters { get; set; }
    }
}