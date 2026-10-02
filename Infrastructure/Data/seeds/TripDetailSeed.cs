using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
    public static class TripDetailSeed
    {
        public static void SeedTripDetails(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TripDetail>().HasData(
                new TripDetail
                {
                    Id = 1,
                    TripId = 1,
                    Jarak = 150,
                    AvgSpeed = 75,
                    Waktu = 7200,
                    ImageUrl = "https://example.com/driver-image.jpg"
                }
            );
        }
    }
}