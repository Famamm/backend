using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
    public static class InspectionItemSeed
    {
        public static void SeedInspectionItems(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InspectionItem>().HasData(
                new InspectionItem { Id = 1, InspectionItemCategoryId = 1, Name = "Ban", IsRequired = true },
                new InspectionItem { Id = 2, InspectionItemCategoryId = 1, Name = "Lampu", IsRequired = true },
                new InspectionItem { Id = 3, InspectionItemCategoryId = 1, Name = "Spion", IsRequired = true },
                new InspectionItem { Id = 4, InspectionItemCategoryId = 1, Name = "Wiper", IsRequired = true },
                new InspectionItem { Id = 5, InspectionItemCategoryId = 1, Name = "Body Kendaraan", IsRequired = true },

                new InspectionItem { Id = 6, InspectionItemCategoryId = 2, Name = "Sabuk pengaman", IsRequired = true },
                new InspectionItem { Id = 7, InspectionItemCategoryId = 2, Name = "Kaca depan", IsRequired = true },
                new InspectionItem { Id = 8, InspectionItemCategoryId = 2, Name = "AC", IsRequired = true },
                new InspectionItem { Id = 9, InspectionItemCategoryId = 2, Name = "Indikator", IsRequired = true },
                new InspectionItem { Id = 10, InspectionItemCategoryId = 2, Name = "Klakson", IsRequired = true },

                new InspectionItem { Id = 11, InspectionItemCategoryId = 3, Name = "Oli mesin", IsRequired = true },
                new InspectionItem { Id = 12, InspectionItemCategoryId = 3, Name = "Air radiator", IsRequired = true },
                new InspectionItem { Id = 13, InspectionItemCategoryId = 3, Name = "Aki", IsRequired = true },
                new InspectionItem { Id = 14, InspectionItemCategoryId = 3, Name = "Selang radiator", IsRequired = true },
                new InspectionItem { Id = 15, InspectionItemCategoryId = 3, Name = "Kebocoran oli", IsRequired = true },

                new InspectionItem { Id = 16, InspectionItemCategoryId = 4, Name = "KIR kendaraan", IsRequired = true },
                new InspectionItem { Id = 17, InspectionItemCategoryId = 4, Name = "Segitiga pengaman", IsRequired = true },
                new InspectionItem { Id = 18, InspectionItemCategoryId = 4, Name = "Kotak P3K", IsRequired = true },
                new InspectionItem { Id = 19, InspectionItemCategoryId = 4, Name = "Ban cadangan", IsRequired = true },
                new InspectionItem { Id = 20, InspectionItemCategoryId = 4, Name = "APAR", IsRequired = true }
            );
        }
    }
}