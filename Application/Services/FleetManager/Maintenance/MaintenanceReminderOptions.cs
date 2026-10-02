namespace fleetops_backend.Application.Services
{
    public class MaintenanceReminderOptions
    {
        public const string SectionName = "MaintenanceReminder";

        // Ambang hari bertingkat, urutan bebas. Contoh: H-7, H-3, H-1.
        public int[] ReminderDays { get; set; } = [7, 3, 1];

        public int KmBefore { get; set; } = 500;
        public int ScanIntervalMinutes { get; set; } = 1440;

        // Ambang "due soon" untuk tampilan API = ambang paling awal.
        public int DaysBefore => ReminderDays.Length == 0 ? 0 : ReminderDays.Max();

        // Tingkat reminder maksimum yang mungkin.
        public int MaxStage => ReminderDays.Length;
    }
}
