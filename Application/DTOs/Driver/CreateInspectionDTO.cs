namespace fleetops_backend.Application.DTOs
{
    public class CreateInspectionDTO
    {
        public required int DriverId { get; set; }
        public required int VehicleId { get; set; }
        public List<InspectionResultInputDTO> Results { get; set; } = new List<InspectionResultInputDTO>();
        public List<IFormFile>? Photos { get; set; }
    }

    public class InspectionResultInputDTO
    {
        public required int ItemId { get; set; }
        public required int Condition { get; set; }
        public string? Note { get; set; }
        public int? PhotoIndex { get; set; }
    }
}
