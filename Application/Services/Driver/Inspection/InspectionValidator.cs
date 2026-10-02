using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class InspectionValidator : IInspectionValidator
    {
        public void ValidateDriverExists(Driver? driver)
        {
            if (driver == null)
            {
                throw new InvalidOperationException("Driver not found.");
            }
        }

        public void ValidateVehicleExists(Vehicle? vehicle)
        {
            if (vehicle == null)
            {
                throw new InvalidOperationException("Vehicle not found.");
            }
        }

        public void ValidateSameOrganization(Driver driver, Vehicle vehicle)
        {
            if (driver.OrganizationId != vehicle.OrganizationId)
            {
                throw new InvalidOperationException("Driver and vehicle must belong to the same organization.");
            }
        }

        public void ValidateOrganizationClaim(int organizationId, int? orgClaimId)
        {
            if (orgClaimId.HasValue && orgClaimId.Value != organizationId)
            {
                throw new InvalidOperationException("Unauthorized organization.");
            }
        }

        public void ValidateNoInspectionToday(Inspection? existingInspection)
        {
            if (existingInspection != null)
            {
                throw new InvalidOperationException("Vehicle already has an inspection today.");
            }
        }

        public void ValidateDriverNotChanged(int existingDriverId, int requestedDriverId)
        {
            if (existingDriverId != requestedDriverId)
            {
                throw new InvalidOperationException("Cannot change driver for existing inspection.");
            }
        }

        public void ValidateVehicleSameOrganizationAsInspection(Vehicle vehicle, int inspectionOrganizationId)
        {
            if (vehicle.OrganizationId != inspectionOrganizationId)
            {
                throw new InvalidOperationException("Vehicle and inspection must belong to the same organization.");
            }
        }

        public (DateTimeOffset StartUtc, DateTimeOffset EndUtc) GetTodayRangeUtc(DateTimeOffset now)
        {
            var offset = TimeSpan.FromHours(7);
            var localStart = now.ToOffset(offset).Date;
            var startUtc = new DateTimeOffset(localStart, offset).ToUniversalTime();
            var endUtc = startUtc.AddDays(1);

            return (startUtc, endUtc);
        }
    }
}
