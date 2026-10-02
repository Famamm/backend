namespace fleetops_backend.Application.DTOs
{
    public class GetTripDTO
    {
        public int TripId { get; set; }
        public required string LokasiAwal {get; set;}
        public required string LokasiAkhir {get; set;}
        public required DateTimeOffset WaktuMulai { get; set; }
        public required DateTimeOffset WaktuSelesai { get; set; }
        public required int JarakTempuh { get; set; }
        public required int Durasi { get; set; }
        public required string Status { get; set; }
    }
}