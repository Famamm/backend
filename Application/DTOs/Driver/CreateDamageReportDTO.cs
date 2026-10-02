using Microsoft.AspNetCore.Http;

namespace fleetops_backend.Application.DTOs
{
    public class CreateDamageReportDTO
    {
        // 0 = Ringan, 1 = Sedang, 2 = Berat
        public required int Severity { get; set; }

        // 0 = Beroperasi, 1 = BerhentiAman, 2 = TidakDapatBeroperasi
        public required int OperationalStatus { get; set; }

        public string? DamageType { get; set; }
        public string? Note { get; set; }

        // Opsional; kalau kosong diisi otomatis dari GPS terakhir / lokasi awal trip.
        public string? Location { get; set; }

        // Opsional; kalau kosong diisi otomatis dari odometer awal trip.
        public int? Odometer { get; set; }

        public IFormFile? Image { get; set; }
    }
}
