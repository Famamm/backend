using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
    public static class OrganizationSeed
    {
        public static void SeedOrganizations(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organization>().HasData(
                new Organization
                {
                    Id = 1,
                    OrganizationName = "FleetOps Organization",
                    CreatedAt = new DateTimeOffset(new DateTime(2026, 5, 29, 9, 38, 19, 734, DateTimeKind.Unspecified).AddTicks(4434), new TimeSpan(0,0,0,0,0))
                }
            );
        }
    }
}
