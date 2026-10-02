namespace fleetops_backend.Application.DTOs
{
    public class DriverMaintenanceStatusDTO
    {
        public required VehicleMaintenanceStatusDTO Summary { get; set; }
        public List<MaintenanceDTO> Maintenances { get; set; } = new();
    }
}
