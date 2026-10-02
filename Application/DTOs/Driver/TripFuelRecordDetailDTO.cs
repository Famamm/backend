using System;

namespace fleetops_backend.Application.DTOs
{
    public class TripFuelRecordDetailDTO
    {
        public int FuelId { get; set; }
        public string FuelType { get; set; } = string.Empty;
        public decimal JmlLiter { get; set; }
        public int HargaTotal { get; set; }
        public DateTime WaktuPengisian { get; set; }
        public string? ImageUrlPath { get; set; } 
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string PlatNomor { get; set; } = string.Empty;
        public int OdometerIsi { get; set; }
        public string Catatan { get; set; } = string.Empty;

        // Lokasi pengisian (null bila driver tidak mengizinkan akses GPS)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}