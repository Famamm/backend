namespace fleetops_backend.Application.DTOs
{
    public class FleetActivityDayDTO
    {
        public DateOnly Tanggal { get; set; }
        public int JumlahPerjalanan { get; set; }
    }

    public class FleetStatisticsDTO
    {
        public int Bulan { get; set; }
        public int Tahun { get; set; }

        public int TotalKendaraan { get; set; }
        public int KendaraanAktif { get; set; }
        public int KendaraanDalamServis { get; set; }
        public int KendaraanTidakAktif { get; set; }

        public List<FleetActivityDayDTO> AktivitasArmada { get; set; } = new();
        public int TotalPerjalanan { get; set; }

        public double TotalJarak { get; set; }
        public decimal TotalBbmLiter { get; set; }
        public long TotalHargaBbm { get; set; }
        public List<string> JenisBbm { get; set; } = new();
    }
}
