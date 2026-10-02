using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
    public static class VehicleSeed
    {
        public static void SeedVehicles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vehicle>().HasData(
                new Vehicle
                {
                    Id = 1,
                    DriverId = 1,
                    OrganizationId = 1,
                    VehicleName = "Truck-001",
                    PlatNomor = "B 1234 ABC",
                    TahunPembuatan = 2024,
                    Odometer = 0,
                    Tipe = "Truck",
                    Status = "Active",
                    MuatanMax = 1000,
                    TipeBensin = TipeBensin.Diesel,
                    ImageUrl = null
                }
            );
        }
    }
}