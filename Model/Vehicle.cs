using System.ComponentModel.DataAnnotations.Schema;


namespace fleetops_backend.Models
{
    public enum TipeBensin
    {
        Diesel = 0,
        Bensin = 1
    }

    public class Vehicle
    {
        public int Id { get; set; }
        
        [Column("driver_id")]
        public int? DriverId { get; set; }
        
        [ForeignKey(nameof(DriverId))]
        public Driver? Driver { get; set; }

        [Column("organization_id")]
        public int OrganizationId { get; set; }

        [ForeignKey(nameof(OrganizationId))]
        public Organization? Organization { get; set; }

        public required string VehicleName {get;set;}
        public required string PlatNomor {get;set;}
        public required int TahunPembuatan {get;set;}
        public required int Odometer {get;set;}
        public required string Tipe {get;set;}
        public required string Status {get;set;}
        public required int MuatanMax{get;set;}
        public required TipeBensin TipeBensin{get;set;}
        public string? ImageUrl{get;set;}
        public ICollection<Fuel> Fuel { get; set; } = new List<Fuel>();
        public ICollection<Maintenance> Maintenance { get; set; } = new List<Maintenance>();
        public ICollection<Trip> Trip { get; set; } = new List<Trip>();
        public ICollection<Inspection> Inspection { get; set; } = new List<Inspection>();
        public ICollection<DamageReport> DamageReports { get; set; } = new List<DamageReport>();
    } 
}

