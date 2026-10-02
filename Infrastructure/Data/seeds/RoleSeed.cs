using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;

namespace fleetops_backend.Infrastructure.Data.Seeds
{
    public static class RoleSeed // Roles -> Users (1 Role -> many Users)
    {
        public static void SeedRoles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "FleetManager" },
                new Role { Id = 3, Name = "Driver" },
                new Role { Id = 4, Name = "Employee" },
                new Role { Id = 5, Name = "Unassigned" }
            );
        }
    }
}