using Microsoft.AspNetCore.Http;
using fleetops_backend.Models;

namespace fleetops_backend.Application.DTOs
{
    public class VehicleDTO
    {
        public required string VehicleName { get; set; }
        public required string PlatNomor { get; set; }
        public required int TahunPembuatan { get; set; }
        public required int Odometer { get; set; }
        public required string Tipe { get; set; }
        public required string Status { get; set; }
        public required int MuatanMax { get; set; }
        public required TipeBensin TipeBensin { get; set; }
        public IFormFile? Image { get; set; }
    }
}
