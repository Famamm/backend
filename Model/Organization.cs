using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public required string OrganizationName { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public ICollection<User> Users { get; set; } = new List<User>();
        public ICollection<Driver> Drivers { get; set; } = new List<Driver>();
        public ICollection<FleetManager> FleetManagers { get; set; } = new List<FleetManager>();
        public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public ICollection<Maintenance> Maintenances { get; set; } = new List<Maintenance>();
        public ICollection<Fuel> Fuels { get; set; } = new List<Fuel>();
        public ICollection<Inspection> Inspections { get; set; } = new List<Inspection>();
        public ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();
    }
}