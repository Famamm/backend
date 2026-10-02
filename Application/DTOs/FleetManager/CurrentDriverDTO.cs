namespace fleetops_backend.Application.DTOs
{
    public class CurrentDriverDTO
    {
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string PlatNomor { get; set; } = string.Empty;
        public string Tipe { get; set; } = string.Empty;
        public int Odometer { get; set; }
        public string VehicleStatus { get; set; } = string.Empty;

        public int? DriverId { get; set; }
        public string? NamaDriver { get; set; }
        public string? Username { get; set; }
        public string? ProfilePhoto { get; set; }
        public string? JenisLisensi { get; set; }
        public string? DriverStatus { get; set; }
    }
}
