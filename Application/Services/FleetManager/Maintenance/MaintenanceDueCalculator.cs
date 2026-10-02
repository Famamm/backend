using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public static class MaintenanceDueCalculator
    {
        public static MaintenanceDueInfo? Evaluate(
            Maintenance maintenance,
            int currentOdometer,
            DateOnly today,
            MaintenanceReminderOptions options)
        {
            if (maintenance.Status != MaintenanceStatus.Scheduled)
            {
                return null;
            }

            int? daysRemaining = maintenance.TanggalJadwal is { } date ? date.DayNumber - today.DayNumber : null;
            int? kmRemaining = maintenance.TargetOdometer is { } target ? target - currentOdometer : null;

            var state = MaintenanceDueState.Upcoming;

            if (daysRemaining < 0 || kmRemaining <= 0)
            {
                state = MaintenanceDueState.Overdue;
            }
            else if (daysRemaining <= options.DaysBefore || kmRemaining <= options.KmBefore)
            {
                state = MaintenanceDueState.DueSoon;
            }

            return new MaintenanceDueInfo(state, daysRemaining, kmRemaining);
        }

        /// <summary>
        /// Tingkat reminder yang seharusnya sudah terkirim untuk kondisi saat ini.
        /// 0 = belum masuk ambang apa pun.
        /// </summary>
        public static int GetReminderStage(MaintenanceDueInfo due, MaintenanceReminderOptions options)
        {
            if (due.State == MaintenanceDueState.Overdue)
            {
                return 0; // ditangani jalur overdue terpisah
            }

            // Tingkat berbasis tanggal: jumlah ambang yang sudah terlewati.
            var dateStage = due.DaysRemaining is { } days
                ? options.ReminderDays.Count(threshold => days <= threshold)
                : 0;

            // Km memicu tingkat pertama saja.
            var kmStage = due.KmRemaining is { } km && km <= options.KmBefore ? 1 : 0;

            return Math.Max(dateStage, kmStage);
        }

        public static Maintenance? SelectNext(
            IEnumerable<Maintenance> maintenances,
            int currentOdometer,
            DateOnly today,
            MaintenanceReminderOptions options)
        {
            return maintenances
                .Where(m => m.Status == MaintenanceStatus.Scheduled)
                .OrderByDescending(m => Evaluate(m, currentOdometer, today, options)!.State)
                .ThenBy(m => m.TanggalJadwal ?? DateOnly.MaxValue)
                .ThenBy(m => m.TargetOdometer ?? int.MaxValue)
                .ThenBy(m => m.Id)
                .FirstOrDefault();
        }
    }
}
