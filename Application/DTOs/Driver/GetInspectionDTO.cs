namespace fleetops_backend.Application.DTOs
{
    public class GetInspectionDTO
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public string? VehicleName { get; set; }
        public string? PlatNomor { get; set; }
        public bool Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class GetDetailInspectionDTO
    {
        public int Id { get; set; }
        public int VehicleId { get; set; }
        public int DriverId { get; set; }
        public string? VehicleName { get; set; }
        public string? PlatNomor { get; set; }
        public bool Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public List<InspectionResultDTO> Results { get; set; } = new List<InspectionResultDTO>();
        public List<InspectionPhotoDTO> Photos { get; set; } = new List<InspectionPhotoDTO>();
    }

    public class InspectionResultDTO
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string? ItemName { get; set; }
        public int Condition { get; set; }
        public string? Note { get; set; }
        public int? PhotoId { get; set; }
    }

    public class InspectionPhotoDTO
    {
        public int Id { get; set; }
        public string ImageUrlPath { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
    }
}
