using fleetops_backend.Models;

namespace fleetops_backend.Application.DTOs
{
	public class CreateVehicleDTO
	{
		public required int OrganizationId { get; set; }
		public required string VehicleName { get; set; }
		public required string PlatNomor { get; set; }
		public required int Odometer { get; set; }
		public required string Tipe { get; set; }
		public required string Status { get; set; }
		public required int MuatanMax { get; set; }
		public required TipeBensin TipeBensin { get; set; }
		public required string ImageUrl { get; set; }
	}
}