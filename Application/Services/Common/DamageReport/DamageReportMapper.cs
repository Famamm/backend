using fleetops_backend.Application.DTOs;
using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public static class DamageReportMapper
    {
        public static DamageReportDTO ToDto(DamageReport report)
        {
            return new DamageReportDTO
            {
                Id = report.Id,
                TripId = report.TripId,
                VehicleId = report.VehicleId,
                VehicleName = report.Vehicle?.VehicleName,
                PlatNomor = report.Vehicle?.PlatNomor,
                DriverId = report.DriverId,
                DriverName = report.Driver?.User?.FullName,
                Severity = report.Severity.ToString(),
                Priority = report.Priority,
                OperationalStatus = report.OperationalStatus.ToString(),
                Status = report.Status.ToString(),
                DamageType = report.DamageType,
                Note = report.Note,
                IncidentAt = report.IncidentAt.ToOffset(TimeSpan.FromHours(7)),
                Location = report.Location,
                Odometer = report.Odometer,
                ImageUrlPath = report.ImageUrlPath,
                ReviewNote = report.ReviewNote,
                ResolvedAt = report.ResolvedAt?.ToOffset(TimeSpan.FromHours(7)),
                CreatedAt = report.CreatedAt.ToOffset(TimeSpan.FromHours(7))
            };
        }
    }
}
