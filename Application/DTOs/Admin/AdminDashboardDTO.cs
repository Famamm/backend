namespace fleetops_backend.Application.DTOs
{
    public class AdminBbmActivityDayDTO
    {
        public DateOnly Tanggal { get; set; }
        public decimal TotalLiter { get; set; }
        public long TotalHarga { get; set; }
    }

    public class AdminJenisBbmDTO
    {
        public string Jenis { get; set; } = string.Empty;
        public decimal TotalLiter { get; set; }
        public long TotalHarga { get; set; }
    }

    public class AdminRincianAktivitasArmadaDTO
    {
        public long TotalJarak { get; set; }
        public long TotalDurasi { get; set; }
        public double RataRataKecepatan { get; set; }
    }

    public class AdminDashboardDTO
    {
        public string OrganizationName { get; set; } = string.Empty;
        public int TotalDrivers { get; set; }
        public int TotalVehicles { get; set; }
        public int TotalTrips { get; set; }

        public int Bulan { get; set; }
        public int Tahun { get; set; }

        /// <summary>Rentang waktu chart Aktivitas Armada: "week", "month", atau "year".</summary>
        public string RangeArmada { get; set; } = "week";

        /// <summary>Rentang waktu chart Biaya BBM: "week", "month", atau "year".</summary>
        public string RangeBbm { get; set; } = "week";

        public List<FleetActivityDayDTO> AktivitasArmada { get; set; } = new();
        public int TotalPerjalanan { get; set; }
        public AdminRincianAktivitasArmadaDTO RincianAktivitasArmada { get; set; } = new();

        public List<AdminBbmActivityDayDTO> AktivitasBbm { get; set; } = new();

        public decimal TotalBbmLiter { get; set; }
        public long TotalHargaBbm { get; set; }
        public List<string> JenisBbm { get; set; } = new();

        public List<AdminJenisBbmDTO> RincianJenisBbm { get; set; } = new();
        public int TotalFleetManagers { get; set; }
    }
}