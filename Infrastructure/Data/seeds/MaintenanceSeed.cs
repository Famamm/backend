using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
    public static class MaintenanceSeed
    {
        public static void SeedMaintenances(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Maintenance>().HasData(
                new Maintenance
                {
                    Id = 1,
                    VehicleId = 1,
                    OrganizationId = 1,
                    Jenis = "Pemeriksaan rutin",
                    Status = MaintenanceStatus.Completed,
                    Tanggal = new DateTime(2026, 5, 2, 11, 59, 13, 651, DateTimeKind.Utc),
                    Catatan = "Rutin maintenance check",
                    Odometer = 5000,
                    Biaya = 500000,
                    CreatedAt = new DateTimeOffset(new DateTime(2026, 5, 2, 11, 59, 13, 651), new TimeSpan(0, 0, 0, 0, 0))
                }
            );
        }
    }
}