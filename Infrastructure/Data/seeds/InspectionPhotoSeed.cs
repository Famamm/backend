using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
    public static class InspectionPhotoSeed
    {
        public static void SeedInspectionPhotos(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<InspectionPhoto>().HasData(
                new InspectionPhoto
                {
                    Id = 1,
                    InspectionId = 1,
                    IsRequired = true,
                    ImageUrlPath = "https://example.com/photo1.jpg"
                }
            );
        }
    }
}