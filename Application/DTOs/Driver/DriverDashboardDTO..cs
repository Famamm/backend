namespace fleetops_backend.Application.DTOs
{
    public class DriverDashboardDTO
    {
        public string DriverName { get; set; } = string.Empty;
        public int TotalJarak { get; set; }
        public int TripSelesai { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string PlatNomor { get; set; } = string.Empty;
        public string VehicleStatus { get; set; } = string.Empty;
        public string OrganizationName { get; set; } = string.Empty;

        public VehicleEfficiencyDTO? FuelEfficiency { get; set; }
    }
}