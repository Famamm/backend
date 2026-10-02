namespace fleetops_backend.Application.DTOs
{
    public class VehicleMaintenanceStatusDTO
    {
        public int VehicleId { get; set; }
        public required string VehicleName { get; set; }
        public required string PlatNomor { get; set; }
        public required string VehicleStatus { get; set; }
        public int Odometer { get; set; }
        public string? DueState { get; set; }
        public MaintenanceDTO? ActiveMaintenance { get; set; }
        public MaintenanceDTO? NextMaintenance { get; set; }
        public MaintenanceDTO? LastCompleted { get; set; }
    }
}
