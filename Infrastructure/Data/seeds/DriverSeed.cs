using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
    public static class DriverSeed
    {
        public static void SeedDrivers(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Driver>().HasData(
                new Driver 
                { 
                    Id = 1,
                    UserId = 1,
                    OrganizationId = 1,
                    JenisLisensi = "B",
                    TanggalBerlaku = new DateTimeOffset(new DateTime(2031, 5, 2, 11, 59, 13, 649), new TimeSpan(0, 0, 0, 0, 0)),
                    FotoLisensi = "https://example.com/sim/driver1.jpg",
                    Status = "Active"
                }
            );
        }
    }
}