namespace fleetops_backend.Application.DTOs
{
    public class GetActiveTripDTO
    {
        public int TripId { get; set; }
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string PlatNomor { get; set; } = string.Empty;
        public string LokasiAwal { get; set; } = string.Empty;
        public string? LokasiAkhir { get; set; }
        public DateTimeOffset WaktuMulai { get; set; }
        public int? OdometerAwal { get; set; }
        public string Status { get; set; } = string.Empty;
        public int DurasiDetik { get; set; }
    }
}