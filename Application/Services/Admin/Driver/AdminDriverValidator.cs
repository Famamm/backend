using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public class AdminDriverValidator : IAdminDriverValidator
    {
        public void ValidateOrganizationId(int organizationId)
        {
            if (organizationId <= 0)
            {
                throw new ArgumentException("Organization ID must be greater than zero.", nameof(organizationId));
            }
        }

        public void ValidateDriverOrganization(Driver driver, int organizationId)
        {
            if (driver.OrganizationId != organizationId)
            {
                throw new KeyNotFoundException("Driver not found.");
            }
        }
    }
}
