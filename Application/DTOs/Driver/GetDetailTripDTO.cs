namespace fleetops_backend.Application.DTOs
{
    public class GetDetailTripDTO
    {
        public int TripId { get; set; }
        public int DriverId { get; set; }
        public int VehicleId { get; set; }
        public string? LokasiAwal { get; set; }
        public string? LokasiAkhir { get; set; }
        public DateTimeOffset WaktuMulai { get; set; }
        public DateTimeOffset? WaktuSelesai { get; set; }
        public string? Catatan { get; set; }
        public double? JarakTempuh { get; set; }
        public List<TripDetailDTO> Details { get; set; } = new();
        public List<TripFuelRecordDTO> RiwayatBbm { get; set; } = new();
    }
}