using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class InspectionMapper : IInspectionMapper
    {
        public GetInspectionDTO ToInspectionDto(Inspection inspection)
        {
            return new GetInspectionDTO
            {
                Id = inspection.Id,
                DriverId = inspection.DriverId,
                VehicleId = inspection.VehicleId,
                VehicleName = inspection.Vehicle?.VehicleName ?? string.Empty,
                PlatNomor = inspection.Vehicle?.PlatNomor ?? string.Empty,
                Status = inspection.Status,
                CreatedAt = inspection.CreatedAt.ToOffset(TimeSpan.FromHours(7)),
            };
        }

        public GetDetailInspectionDTO ToDetailInspectionDto(Inspection inspection)
        {
            return new GetDetailInspectionDTO
            {
                Id = inspection.Id,
                DriverId = inspection.DriverId,
                VehicleId = inspection.VehicleId,
                VehicleName = inspection.Vehicle?.VehicleName ?? string.Empty,
                PlatNomor = inspection.Vehicle?.PlatNomor ?? string.Empty,
                Status = inspection.Status,
                CreatedAt = inspection.CreatedAt.ToOffset(TimeSpan.FromHours(7)),
                Results = inspection.Results.Select(r => new InspectionResultDTO
                {
                    Id = r.Id,
                    ItemId = r.ItemId,
                    ItemName = r.Item?.Name,
                    Condition = (int)r.Condition,
                    Note = r.Note,
                    PhotoId = r.PhotoId
                }).ToList(),
                Photos = inspection.Photos.Select(p => new InspectionPhotoDTO
                {
                    Id = p.Id,
                    ImageUrlPath = p.ImageUrlPath,
                    IsRequired = p.IsRequired
                }).ToList()
            };
        }
    }
}
