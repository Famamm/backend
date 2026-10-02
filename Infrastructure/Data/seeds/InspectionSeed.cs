using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
    public static class InspectionSeed
    {
        public static void SeedInspections(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inspection>().HasData(
                new Inspection
                {
                    Id = 1,
                    DriverId = 1,
                    VehicleId = 1,
                    OrganizationId = 1,
                    Status = true,
                    CreatedAt = new DateTimeOffset(new DateTime(2026, 5, 2, 11, 59, 13, 652), new TimeSpan(0, 0, 0, 0, 0))
                }
            );
        }
    }
}