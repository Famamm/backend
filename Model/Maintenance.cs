using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public class Maintenance
    {
        public int Id { get; set; }

        [Column("vehicle_id")]
        public int VehicleId { get; set; }
        public Vehicle? Vehicle { get; set; }

        [Column("organization_id")]
        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public required string Jenis { get; set; }
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Scheduled;

        public DateOnly? TanggalJadwal { get; set; }
        public int? TargetOdometer { get; set; }

        // Data realisasi, terisi saat status menjadi Completed.
        public DateTime? Tanggal { get; set; }
        public string? Catatan { get; set; }
        public int? Odometer { get; set; }
        public decimal? Biaya { get; set; }

        public DateTimeOffset? ReminderSentAt { get; set; }
        public DateTimeOffset? OverdueNotifiedAt { get; set; }

        // Jumlah tingkat reminder yang sudah terkirim (0 = belum ada).
        public int ReminderStage { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
