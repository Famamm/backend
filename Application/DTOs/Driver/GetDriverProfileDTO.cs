using fleetops_backend.Models;

namespace fleetops_backend.Application.DTOs
{
    public class DriverVehicleDto
    {
        public int VehicleId { get; set; }
        public required string VehicleName { get; set; }
        public required string PlatNomor { get; set; }
        public required string Tipe { get; set; }
        public TipeBensin TipeBensin { get; set; }
        public required string Status { get; set; }
        public string? ImageUrl { get; set; }
        public int Odometer { get; set; }
    }

    public class DriverDocumentDto
    {
        public required string JenisLisensi { get; set; }
        public string? FotoLisensi { get; set; }
        public required DateTimeOffset TanggalBerlaku { get; set; }
        public required string Status { get; set; }
    }

    public class DriverStatisticsDto
    {
        public double TotalJarak { get; set; }
        public int TotalTrip { get; set; }
        public double TotalJamMengemudi { get; set; }
        public double RataRataKecepatan { get; set; }
    }

    public class GetDriverProfileDTO
    {
        public int OrganizationId { get; set; }
        public string? OrganizationName { get; set; }
        public int DriverId { get; set; }
        public required string NamaDriver { get; set; }
        public required string Email { get; set; }
        public string? Address { get; set; }
        public string? PhotoUrl { get; set; }
        public required DriverStatisticsDto Statistics { get; set; }
        public DriverVehicleDto? KendaraanAktif { get; set; }
        public List<DriverVehicleDto> SemuaKendaraan { get; set; } = new();
        public List<DriverVehicleDto> KendaraanTersedia { get; set; } = new();
        public DriverDocumentDto? Dokumen { get; set; }
    }
}
