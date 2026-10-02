using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IAdminDriverValidator
    {
        void ValidateOrganizationId(int organizationId);
        void ValidateDriverOrganization(Driver driver, int organizationId);
    }
}
