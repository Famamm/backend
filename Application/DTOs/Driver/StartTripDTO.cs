namespace fleetops_backend.Application.DTOs
{
	public class StartTripDTO
	{
		public required int DriverId { get; set; }
		public required int VehicleId { get; set; }
		public string? LokasiAwal { get; set; }
		public string? LokasiAkhir { get; set; }
		public double? KordAwalLatitude { get; set; }
		public double? KordAwalLongitude { get; set; }
		public int? OdometerAwal { get; set; }
	}
}
