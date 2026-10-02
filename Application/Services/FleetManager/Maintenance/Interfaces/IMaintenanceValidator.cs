using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IMaintenanceValidator
    {
        void ValidateVehicleSchedulable(Vehicle vehicle);
        void ValidateSchedule(MaintenanceScheduleDTO dto, int vehicleOdometer, DateOnly today);
        void ValidateEditable(Maintenance maintenance);
        MaintenanceStatus ParseStatus(string? value);
        void ValidateTransition(MaintenanceStatus from, MaintenanceStatus to);
        void ValidateCompletion(UpdateMaintenanceStatusDTO dto);
    }
}
