using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IInspectionValidator
    {
        void ValidateDriverExists(Driver? driver);
        void ValidateVehicleExists(Vehicle? vehicle);
        void ValidateSameOrganization(Driver driver, Vehicle vehicle);
        void ValidateOrganizationClaim(int organizationId, int? orgClaimId);
        void ValidateNoInspectionToday(Inspection? existingInspection);
        void ValidateDriverNotChanged(int existingDriverId, int requestedDriverId);
        void ValidateVehicleSameOrganizationAsInspection(Vehicle vehicle, int inspectionOrganizationId);
        (DateTimeOffset StartUtc, DateTimeOffset EndUtc) GetTodayRangeUtc(DateTimeOffset now);
    }
}

