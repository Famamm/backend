using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;
using NetTopologySuite.Geometries;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
    public static class FuelSeed
    {
        public static void SeedFuels(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fuel>().HasData(
                new Fuel
                {
                    Id = 1,
                    VehicleId = 1,
                    DriverId = 1,
                    OrganizationId = 1,
                    JmlLiter = 50,
                    FuelPriceId = 3,
                    HargaTotal = 750000,
                    LokasiPengisian = new Point(106.8272, -6.1751) { SRID = 4326 },
                    CreatedAt = new DateTime(2026, 5, 2, 11, 59, 13, 651, DateTimeKind.Utc)
                }
            );
        }
    }
}