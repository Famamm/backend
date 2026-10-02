using System.Globalization;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public static class NotificationMessageBuilder
    {
        public static NotificationContent BuildFuelAnomaly(string plateNumber, int fuelId, FuelAnomaly anomaly)
        {
            var percentage = Math.Abs(anomaly.Deviation) * 100;
            var direction = anomaly.Deviation < 0 ? "lebih boros" : "lebih hemat";

            var message = string.Create(
                CultureInfo.InvariantCulture,
                $"Konsumsi BBM {plateNumber} tercatat {anomaly.KmPerLiter:0.0} km/L, {percentage:0}% {direction} dari rata-rata {anomaly.BaselineKmPerLiter:0.0} km/L.");

            return new NotificationContent(NotificationType.FuelAnomaly, "Anomali konsumsi BBM", message, fuelId);
        }

        public static NotificationContent BuildDamageReport(
            string driverName,
            string plateNumber,
            int inspectionId,
            int damagedItemCount)
        {
            var message = $"{driverName} melaporkan {damagedItemCount} item bermasalah pada {plateNumber}.";

            return new NotificationContent(NotificationType.DamageReport, "Laporan kerusakan kendaraan", message, inspectionId);
        }

        public static NotificationContent BuildTripDamageReport(
            string driverName,
            string plateNumber,
            int damageReportId,
            DamageSeverity severity,
            OperationalStatus operationalStatus)
        {
            var message = $"{driverName} melaporkan kerusakan {severity} pada {plateNumber} ({operationalStatus}).";

            return new NotificationContent(NotificationType.TripDamageReport, "Laporan kerusakan kendaraan", message, damageReportId);
        }

        public static NotificationContent BuildTripDamageReportStatusChanged(
            string plateNumber,
            int damageReportId,
            DamageReportStatus status)
        {
            var statusLabel = status switch
            {
                DamageReportStatus.InReview => "sedang ditinjau",
                DamageReportStatus.InRepair => "sedang diperbaiki",
                DamageReportStatus.Resolved => "selesai ditangani",
                DamageReportStatus.Rejected => "ditolak",
                _ => "dilaporkan"
            };
            var message = $"Laporan kerusakan {plateNumber} {statusLabel}.";

            return new NotificationContent(NotificationType.TripDamageReportStatusChanged, "Status laporan kerusakan diperbarui", message, damageReportId);
        }

        public static NotificationContent BuildMaintenanceReminder(
            string plateNumber,
            string jenis,
            int maintenanceId,
            MaintenanceDueInfo due)
        {
            var parts = new List<string>();

            if (due.DaysRemaining is { } days)
            {
                parts.Add(days == 0 ? "hari ini" : $"dalam {days} hari");
            }

            if (due.KmRemaining is { } km)
            {
                parts.Add(string.Create(CultureInfo.InvariantCulture, $"dalam {km} km"));
            }

            var when = string.Join(" atau ", parts);
            var suffix = parts.Count > 1 ? ", mana yang tercapai lebih dulu" : "";
            var message = $"Pemeliharaan {jenis} untuk {plateNumber} jatuh tempo {when}{suffix}.";

            return new NotificationContent(NotificationType.MaintenanceReminder, "Pengingat pemeliharaan", message, maintenanceId);
        }

        public static NotificationContent BuildMaintenanceOverdue(string plateNumber, string jenis, int maintenanceId)
        {
            var message = $"Pemeliharaan {jenis} untuk {plateNumber} sudah melewati jatuh tempo.";

            return new NotificationContent(NotificationType.MaintenanceOverdue, "Pemeliharaan terlambat", message, maintenanceId);
        }

        public static NotificationContent BuildMaintenanceStatusChanged(
            string plateNumber,
            string jenis,
            int maintenanceId,
            MaintenanceStatus status)
        {
            var statusLabel = status switch
            {
                MaintenanceStatus.InProgress => "sedang dikerjakan",
                MaintenanceStatus.Completed => "selesai",
                MaintenanceStatus.Cancelled => "dibatalkan",
                _ => "dijadwalkan"
            };
            var message = $"Pemeliharaan {jenis} untuk {plateNumber} {statusLabel}.";

            return new NotificationContent(NotificationType.MaintenanceStatusChanged, "Status pemeliharaan diperbarui", message, maintenanceId);
        }
    }
}
