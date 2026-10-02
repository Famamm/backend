using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
    public static class TripSeed
    {
        public static void SeedTrips(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trip>().HasData(
                new Trip
                {
                    Id = 1,
                    DriverId = 1,
                    VehicleId = 1,
                    OrganizationId = 1,
                    LokasiAwal = "Jakarta",
                    LokasiAkhir = "Bandung",
                    OdometerAwal = 1000,
                    OdometerAkhir = 1150,
                    WaktuMulai = new DateTimeOffset(new DateTime(2026, 5, 2, 9, 59, 13, 650), new TimeSpan(0, 0, 0, 0, 0)),
                    WaktuSelesai = new DateTimeOffset(new DateTime(2026, 5, 2, 11, 59, 13, 650), new TimeSpan(0, 0, 0, 0, 0)),
                    Status = "Finished",
                    Catatan = "Trip completed successfully"
                }
            );
        }
    }
}