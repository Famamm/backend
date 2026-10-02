namespace fleetops_backend.Application.DTOs
{
    public class GetVehicleDTO
    {
        // Vehicle Detail
        public int VehicleId { get; set; }
        public string VehicleName { get; set; } = string.Empty;
        public string PlatNomor { get; set; } = string.Empty;
        public string Tipe { get; set; } = string.Empty;
        public string TipeBensin { get; set; } = string.Empty;
        public int Odometer { get; set; }
        public int MuatanMax { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TahunPembuatan { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        public double? TotalJarak { get; set; }
        public double? TotalBensin { get; set; }
        public double? SkorEfisiensi { get; set; }
        public double? VsBulanLalu { get; set; }
        public string? StatusEfisiensi { get; set; }
    }
}