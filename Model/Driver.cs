using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace fleetops_backend.Models
{
    public class Driver
    {
        public int Id { get; set; }
        
        [Column("user_id")]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Column("organization_id")]
        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public string? JenisLisensi { get; set; }
        public DateTimeOffset? TanggalBerlaku { get; set; }
        public string? FotoLisensi { get; set; }
        public string? Status { get; set; }

        public ICollection<Vehicle> Vehicle { get; set; } = new List<Vehicle>();
        public ICollection<Fuel> Fuel { get; set; } = new List<Fuel>();
        public ICollection<Trip> Trip { get; set; } = new List<Trip>();
        public ICollection<Inspection> Inspection { get; set; } = new List<Inspection>();
        public ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();
    } 
}
