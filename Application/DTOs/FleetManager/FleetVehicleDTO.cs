namespace fleetops_backend.Application.DTOs
{
    public class FleetVehicleDTO
    {
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string PlatNomor { get; set; } = string.Empty;
        public string Tipe { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Odometer { get; set; }
        public string? ImageUrl { get; set; }
        public int? DriverId { get; set; }
        public string? NamaDriver { get; set; }
    }
}
