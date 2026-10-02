using System;

namespace fleetops_backend.Application.DTOs
{
    public class TripFuelRecordDTO
    {
        public int FuelId { get; set; }
        public string FuelType { get; set; } = string.Empty;
        public decimal JmlLiter { get; set; }
        public int HargaTotal { get; set; }
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string PlatNomor { get; set; } = string.Empty;
        public DateTime WaktuPengisian { get; set; }
    }
}