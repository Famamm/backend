using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
	public static class UserSeed
	{
		public static void SeedUsers(this ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().HasData(
				new User
				{
					Id = 1,
					FullName = "FleetOps Test Driver",
					Username = "fleetops.test.driver",
					Email = "driver@fleetops.local",
					Address = "Jl. Test No. 1, Jakarta",
					ProfilePhoto = "https://example.com/profile/driver1.jpg",
					PasswordHash = "$2a$11$wvBoSd9TRmWoarbcfmSrwu.AeqYvmWSP1o.fdUrj88yS33TSJL2Dm",
					RoleId = 3,
					OrganizationId = 1,
					CreatedAt = new DateTimeOffset(new DateTime(2026, 5, 2, 11, 59, 13, DateTimeKind.Utc))
				}
			);
		}
	}
}
