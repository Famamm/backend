using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IInspectionMapper
    {
        GetInspectionDTO ToInspectionDto(Inspection inspection);
        GetDetailInspectionDTO ToDetailInspectionDto(Inspection inspection);
    }
}

