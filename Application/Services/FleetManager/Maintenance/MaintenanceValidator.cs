using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class MaintenanceValidator : IMaintenanceValidator
    {
        private const int MaxJenisLength = 100;

        private static readonly Dictionary<MaintenanceStatus, MaintenanceStatus[]> AllowedTransitions = new()
        {
            [MaintenanceStatus.Scheduled] = [MaintenanceStatus.InProgress, MaintenanceStatus.Cancelled],
            [MaintenanceStatus.InProgress] = [MaintenanceStatus.Completed, MaintenanceStatus.Cancelled],
            [MaintenanceStatus.Completed] = [],
            [MaintenanceStatus.Cancelled] = []
        };

        public void ValidateVehicleSchedulable(Vehicle vehicle)
        {
            if (vehicle.Status != "Active" && vehicle.Status != "Maintenance")
            {
                throw new InvalidOperationException("Kendaraan tidak dapat dijadwalkan pemeliharaan karena statusnya bukan Active.");
            }
        }

        public void ValidateSchedule(MaintenanceScheduleDTO dto, int vehicleOdometer, DateOnly today)
        {
            if (string.IsNullOrWhiteSpace(dto.Jenis))
            {
                throw new InvalidOperationException("Jenis pemeliharaan wajib diisi.");
            }

            if (dto.Jenis.Trim().Length > MaxJenisLength)
            {
                throw new InvalidOperationException($"Jenis pemeliharaan maksimal {MaxJenisLength} karakter.");
            }

            if (dto.TanggalJadwal == null && dto.TargetOdometer == null)
            {
                throw new InvalidOperationException("Isi minimal salah satu dari tanggal jadwal atau target odometer.");
            }

            if (dto.TanggalJadwal < today)
            {
                throw new InvalidOperationException("Tanggal jadwal tidak boleh di masa lalu.");
            }

            if (dto.TargetOdometer <= vehicleOdometer)
            {
                throw new InvalidOperationException(
                    $"Target odometer harus lebih besar dari odometer kendaraan saat ini ({vehicleOdometer} km).");
            }
        }

        public void ValidateEditable(Maintenance maintenance)
        {
            if (maintenance.Status != MaintenanceStatus.Scheduled)
            {
                throw new InvalidOperationException("Hanya jadwal berstatus Scheduled yang dapat diubah atau dihapus.");
            }
        }

        public MaintenanceStatus ParseStatus(string? value)
        {
            if (!Enum.TryParse<MaintenanceStatus>(value, ignoreCase: true, out var status)
                || !Enum.IsDefined(status))
            {
                throw new InvalidOperationException(
                    $"Status tidak valid. Gunakan salah satu dari: {string.Join(", ", Enum.GetNames<MaintenanceStatus>())}.");
            }

            return status;
        }

        public void ValidateTransition(MaintenanceStatus from, MaintenanceStatus to)
        {
            if (!AllowedTransitions[from].Contains(to))
            {
                throw new InvalidOperationException($"Status tidak dapat diubah dari {from} ke {to}.");
            }
        }

        public void ValidateCompletion(UpdateMaintenanceStatusDTO dto)
        {
            if (dto.Odometer < 0)
            {
                throw new InvalidOperationException("Odometer tidak boleh negatif.");
            }

            if (dto.Biaya < 0)
            {
                throw new InvalidOperationException("Biaya tidak boleh negatif.");
            }
        }
    }
}
