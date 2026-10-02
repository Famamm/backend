using fleetops_backend.Models;

namespace fleetops_backend.Application.DTOs
{
    public class UpdateVehicleDTO
    {
        public string? VehicleName { get; set; }
        public string? PlatNomor { get; set; }
        public int? TahunPembuatan { get; set; }
        public int? Odometer { get; set; }
        public string? Tipe { get; set; }
        public string? Status { get; set; }
        public int? MuatanMax { get; set; }
        public TipeBensin? TipeBensin { get; set; }
        public IFormFile? Image { get; set; }
    }
}
