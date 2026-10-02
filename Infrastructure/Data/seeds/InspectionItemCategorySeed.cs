using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
    public static class InspectionItemCategorySeed
    {
        public static void SeedInspectionItemCategories(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InspectionItemCategory>().HasData(
                new InspectionItemCategory { Id = 1, Name = "Eksterior" },
                new InspectionItemCategory { Id = 2, Name = "Interior" },
                new InspectionItemCategory { Id = 3, Name = "Mesin" },
                new InspectionItemCategory { Id = 4, Name = "Kelengkapan" }
            );
        }
    }
}