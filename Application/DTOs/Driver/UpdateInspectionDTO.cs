namespace fleetops_backend.Application.DTOs
{
    public class UpdateInspectionDTO
    {
        public required int DriverId { get; set; }
        public required int VehicleId { get; set; }
        public List<InspectionResultInputDTO> Results { get; set; } = new List<InspectionResultInputDTO>();
        public List<IFormFile>? Photos { get; set; }
    }
}
