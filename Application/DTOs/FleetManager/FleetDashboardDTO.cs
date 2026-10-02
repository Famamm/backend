namespace fleetops_backend.Application.DTOs
{
    public class FleetDashboardDTO
    {
        public int TotalKendaraan { get; set; }
        public int KendaraanAktif { get; set; }
        public int KendaraanBelumAdaDriver { get; set; }
        public int TotalDriver { get; set; }
        public int DriverBelumDitugaskan { get; set; }
    }
}
