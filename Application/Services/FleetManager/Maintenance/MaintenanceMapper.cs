using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public static class MaintenanceMapper
    {
        public static MaintenanceDTO ToDto(
            Maintenance maintenance,
            Vehicle vehicle,
            DateOnly today,
            MaintenanceReminderOptions options)
        {
            var due = MaintenanceDueCalculator.Evaluate(maintenance, vehicle.Odometer, today, options);

            return new MaintenanceDTO
            {
                Id = maintenance.Id,
                VehicleId = maintenance.VehicleId,
                VehicleName = vehicle.VehicleName,
                PlatNomor = vehicle.PlatNomor,
                Jenis = maintenance.Jenis,
                Status = maintenance.Status.ToString(),
                DueState = due?.State.ToString(),
                TanggalJadwal = maintenance.TanggalJadwal,
                TargetOdometer = maintenance.TargetOdometer,
                DaysRemaining = due?.DaysRemaining,
                KmRemaining = due?.KmRemaining,
                Tanggal = maintenance.Tanggal,
                Odometer = maintenance.Odometer,
                Biaya = maintenance.Biaya,
                Catatan = maintenance.Catatan,
                CreatedAt = maintenance.CreatedAt
            };
        }

        public static VehicleMaintenanceStatusDTO ToVehicleStatus(
            Vehicle vehicle,
            IReadOnlyCollection<Maintenance> openMaintenances,
            Maintenance? lastCompleted,
            DateOnly today,
            MaintenanceReminderOptions options)
        {
            var active = openMaintenances.FirstOrDefault(m => m.Status == MaintenanceStatus.InProgress);
            var next = MaintenanceDueCalculator.SelectNext(openMaintenances, vehicle.Odometer, today, options);
            var nextDto = next == null ? null : ToDto(next, vehicle, today, options);

            return new VehicleMaintenanceStatusDTO
            {
                VehicleId = vehicle.Id,
                VehicleName = vehicle.VehicleName,
                PlatNomor = vehicle.PlatNomor,
                VehicleStatus = vehicle.Status,
                Odometer = vehicle.Odometer,
                DueState = nextDto?.DueState,
                ActiveMaintenance = active == null ? null : ToDto(active, vehicle, today, options),
                NextMaintenance = nextDto,
                LastCompleted = lastCompleted == null ? null : ToDto(lastCompleted, vehicle, today, options)
            };
        }
    }
}
