using Microsoft.EntityFrameworkCore;
using fleetops_backend.Models;
using fleetops_backend.Infrastructure.Data.Seeds;

namespace fleetops_backend.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Organization> Organizations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<FleetManager> FleetManagers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Fuel> Fuels { get; set; }
        public DbSet<FuelPrice> FuelPrices { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<TripDetail> TripDetails { get; set; }
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<InspectionItemCategory> InspectionItemCategories { get; set; }
        public DbSet<InspectionItem> InspectionItems { get; set; }
        public DbSet<InspectionPhoto> InspectionPhotos { get; set; }
        public DbSet<InspectionResult> InspectionResults { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<TripGpsPoint> TripGpsPoints { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<DamageReport> DamageReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Trip>()
                .Property(t => t.KordAwal)
                .HasColumnType("geography (Point, 4326)");

            modelBuilder.Entity<Trip>()
                .Property(t => t.KordAkhir)
                .HasColumnType("geography (Point, 4326)");

            modelBuilder.Entity<Fuel>()
                .Property(f => f.LokasiPengisian)
                .HasColumnType("geography (Point, 4326)");

            modelBuilder.SeedRoles();
            modelBuilder.SeedInspectionItemCategories();
            modelBuilder.SeedInspectionItems();
            modelBuilder.SeedOrganizations();

            modelBuilder.SeedUsers();
            modelBuilder.SeedDrivers();
            modelBuilder.SeedVehicles();
            modelBuilder.SeedFuels();
            modelBuilder.SeedMaintenances();
            modelBuilder.SeedTrips();
            modelBuilder.SeedTripDetails();
            modelBuilder.SeedInspections();
            modelBuilder.SeedInspectionPhotos();
            modelBuilder.SeedInspectionResults();

            // Unique index on UsernameD
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Organization -> Users
            modelBuilder.Entity<User>()
                .HasOne(u => u.Organization)
                .WithMany(o => o.Users)
                .HasForeignKey(u => u.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization -> Drivers
            modelBuilder.Entity<Driver>()
                .HasOne(d => d.Organization)
                .WithMany(o => o.Drivers)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization -> FleetManagers
            modelBuilder.Entity<FleetManager>()
                .HasOne(fm => fm.Organization)
                .WithMany(o => o.FleetManagers)
                .HasForeignKey(fm => fm.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization -> Vehicles
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Organization)
                .WithMany(o => o.Vehicles)
                .HasForeignKey(v => v.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization -> Trips
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Organization)
                .WithMany(o => o.Trips)
                .HasForeignKey(t => t.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization -> Maintenances
            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.Organization)
                .WithMany(o => o.Maintenances)
                .HasForeignKey(m => m.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization -> Fuels
            modelBuilder.Entity<Fuel>()
                .HasOne(f => f.Organization)
                .WithMany(o => o.Fuels)
                .HasForeignKey(f => f.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Organization -> Inspections
            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.Organization)
                .WithMany(o => o.Inspections)
                .HasForeignKey(i => i.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add ICollection<Users> Users in Roles if you want two-way nav
            modelBuilder.Entity<Role>()
                .HasMany(r => r.Users)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Users -> Drivers (one-to-one: Driver is profile of User)
            modelBuilder.Entity<Driver>()
                .HasOne(d => d.User)
                .WithOne(u => u.Driver)
                .HasForeignKey<Driver>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Users -> FleetManagers (one-to-one: FleetManager is profile of User)
            modelBuilder.Entity<FleetManager>()
                .HasOne(fm => fm.User)
                .WithOne(u => u.FleetManager)
                .HasForeignKey<FleetManager>(fm => fm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Drivers -> Vehicles (1 Driver -> many Vehicles)
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Driver)
                .WithMany(d => d.Vehicle)
                .HasForeignKey(v => v.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Vehicles -> Fuels (1 Vehicle -> many Fuels)
            modelBuilder.Entity<Fuel>()
                .HasOne(f => f.Vehicle)
                .WithMany(v => v.Fuel)
                .HasForeignKey(f => f.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Fuels -> Drivers (optional link)
            modelBuilder.Entity<Fuel>()
                .HasOne(f => f.Driver)
                .WithMany(d => d.Fuel)
                .HasForeignKey(f => f.DriverId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Vehicles -> Maintenances (1 Vehicle -> many Maintenances)
            modelBuilder.Entity<Maintenance>()
                .HasOne(m => m.Vehicle)
                .WithMany(v => v.Maintenance)
                .HasForeignKey(m => m.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Maintenance>()
                .Property(m => m.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Maintenance>()
                .HasIndex(m => new { m.VehicleId, m.Status });

            // Trips -> Drivers (many Trips -> one Driver)
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Driver)
                .WithMany(d => d.Trip)
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Trips -> Vehicles (many Trips -> one Vehicle)
            modelBuilder.Entity<Trip>()
                .HasOne(t => t.Vehicle)
                .WithMany(v => v.Trip)
                .HasForeignKey(t => t.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // TripDetails -> Trips (1 Trip -> many TripDetails)
            modelBuilder.Entity<TripDetail>()
                .HasOne(td => td.Trip)
                .WithMany(t => t.Detail)
                .HasForeignKey(td => td.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TripGpsPoint>()
                .Property(x => x.Coordinate)
                .HasColumnType("geography (Point, 4326)");

            modelBuilder.Entity<TripGpsPoint>()
                .HasOne(x => x.Trip)
                .WithMany(x => x.GpsPoints)
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TripGpsPoint>()
                .HasIndex(x => new { x.TripId, x.ClientId })
                .IsUnique();

            // Inspections -> Drivers & Vehicles
            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.Driver)
                .WithMany(d => d.Inspection)
                .HasForeignKey(i => i.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.Vehicle)
                .WithMany(v => v.Inspection)
                .HasForeignKey(i => i.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // InspectionPhoto -> Inspections (1 Inspection -> many Photos)
            modelBuilder.Entity<InspectionPhoto>()
                .HasOne(p => p.Inspection)
                .WithMany(i => i.Photos)
                .HasForeignKey(p => p.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // InspectionResult -> Inspections (1 Inspection -> many Results)
            modelBuilder.Entity<InspectionResult>()
                .HasOne(r => r.Inspection)
                .WithMany(i => i.Results)
                .HasForeignKey(r => r.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // InspectionResult -> InspectionItem (many Results -> one Item)
            modelBuilder.Entity<InspectionResult>()
                .HasOne(r => r.Item)
                .WithMany(it => it.Result)
                .HasForeignKey(r => r.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // InspectionItem -> InspectionItemCategory (many Items -> one Category)
            modelBuilder.Entity<InspectionItem>()
                .HasOne(i => i.Category)
                .WithMany(c => c.Items)
                .HasForeignKey(i => i.InspectionItemCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // InspectionResult -> InspectionPhoto (optional)
            modelBuilder.Entity<InspectionResult>()
                .HasOne(r => r.Photo)
                .WithMany(p => p.Result)
                .HasForeignKey(r => r.PhotoId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // RefreshToken -> User (many RefreshTokens -> one User)
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Notification>()
                .Property(n => n.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.UserId, n.IsRead });

            // DamageReport -> Trip (1 Trip -> many DamageReports)
            modelBuilder.Entity<DamageReport>()
                .HasOne(d => d.Trip)
                .WithMany(t => t.DamageReports)
                .HasForeignKey(d => d.TripId)
                .OnDelete(DeleteBehavior.Cascade);

            // DamageReport -> Driver
            modelBuilder.Entity<DamageReport>()
                .HasOne(d => d.Driver)
                .WithMany(dr => dr.DamageReports)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // DamageReport -> Vehicle
            modelBuilder.Entity<DamageReport>()
                .HasOne(d => d.Vehicle)
                .WithMany(v => v.DamageReports)
                .HasForeignKey(d => d.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            // DamageReport -> Organization
            modelBuilder.Entity<DamageReport>()
                .HasOne(d => d.Organization)
                .WithMany(o => o.DamageReports)
                .HasForeignKey(d => d.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DamageReport>()
                .Property(d => d.Severity)
                .HasConversion<string>();

            modelBuilder.Entity<DamageReport>()
                .Property(d => d.OperationalStatus)
                .HasConversion<string>();

            modelBuilder.Entity<DamageReport>()
                .Property(d => d.Status)
                .HasConversion<string>();

            modelBuilder.Entity<DamageReport>()
                .HasIndex(d => new { d.OrganizationId, d.Status });

            // Seed FuelPrices
            FuelPriceSeed.SeedFuelPrices(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }
    }
}

