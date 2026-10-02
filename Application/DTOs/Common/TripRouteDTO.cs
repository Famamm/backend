namespace fleetops_backend.Application.DTOs
{
    public class TripRouteDTO
    {
        public int TripId { get; set; }
        public int DriverId { get; set; }
        public string? DriverName { get; set; }
        public int VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public string? PlatNomor { get; set; }

        public required string Status { get; set; }
        public string? LokasiAwal { get; set; }
        public string? LokasiAkhir { get; set; }
        public DateTimeOffset WaktuMulai { get; set; }
        public DateTimeOffset? WaktuSelesai { get; set; }

        public int PointCount { get; set; }

        // Nama field mengikuti entity GpsPoint di mobile supaya widget peta
        // yang sudah ada (TripLiveMap) bisa langsung memakai response ini.
        public List<TripGpsPointDTO> Points { get; set; } = new();
    }

    public class DriverLiveLocationDTO
    {
        public int TripId { get; set; }
        public int DriverId { get; set; }
        public string? DriverName { get; set; }
        public int VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public string? PlatNomor { get; set; }

        public string? LokasiAwal { get; set; }
        public DateTimeOffset WaktuMulai { get; set; }

        // Null saat trip sudah dimulai tapi belum ada satu pun titik GPS yang
        // masuk — misalnya GPS driver mati. Mobile menampilkannya sebagai
        // driver tanpa posisi, bukan menyembunyikannya dari daftar.
        public TripGpsPointDTO? LastPoint { get; set; }
    }
}
