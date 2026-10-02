using System.Security.Claims;

namespace fleetops_backend.Application.Services
{
    public static class ClaimsHelper
    {
        public static int? GetOrganizationIdOrNull(ClaimsPrincipal? user)
        {
            var orgClaim = user?.FindFirst("OrganizationId");
            if (orgClaim == null || !int.TryParse(orgClaim.Value, out int organizationId))
            {
                return null;
            }

            return organizationId;
        }

        public static int GetUserIdOrThrow(ClaimsPrincipal? user)
        {
            var userIdClaim = user?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new InvalidOperationException("User ID tidak ditemukan dalam token.");
            }

            return userId;
        }

        public static int GetOrganizationIdOrThrow(ClaimsPrincipal? user)
        {
            var orgClaim = user?.FindFirst("OrganizationId");
            if (orgClaim == null || !int.TryParse(orgClaim.Value, out int organizationId))
            {
                throw new InvalidOperationException("Organization ID tidak ditemukan dalam token.");
            }

            return organizationId;
        }
    }
}
