using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
    public static class FuelPriceSeed
    {
        // Tanggal tetap (bukan DateTime.UtcNow) supaya seed deterministik
        // dan tidak memicu UpdateData palsu di setiap migration baru.
        private static readonly DateTime SeedCreatedAt =
            new DateTime(2026, 5, 2, 11, 59, 13, 651, DateTimeKind.Utc);

        public static void SeedFuelPrices(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FuelPrice>().HasData(
                new FuelPrice { Id = 1, FuelType = "Pertalite", PricePerLiter = 10000, CreatedAt = SeedCreatedAt },
                new FuelPrice { Id = 2, FuelType = "BioSolar", PricePerLiter = 6800, CreatedAt = SeedCreatedAt },
                new FuelPrice { Id = 3, FuelType = "Pertamax (RON 92)", PricePerLiter = 16250, CreatedAt = SeedCreatedAt },
                new FuelPrice { Id = 4, FuelType = "Pertamax Green 95", PricePerLiter = 17000, CreatedAt = SeedCreatedAt },
                new FuelPrice { Id = 5, FuelType = "Pertamax Turbo (RON 98)", PricePerLiter = 20750, CreatedAt = SeedCreatedAt },
                new FuelPrice { Id = 6, FuelType = "Dexlite", PricePerLiter = 23000, CreatedAt = SeedCreatedAt },
                new FuelPrice { Id = 7, FuelType = "Pertamina Dex", PricePerLiter = 27900, CreatedAt = SeedCreatedAt }
            );
        }
    }
}