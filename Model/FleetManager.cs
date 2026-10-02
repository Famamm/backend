using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public class FleetManager
    {
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Column("organization_id")]
        public int OrganizationId { get; set; }
        public Organization? Organization { get; set; }

        public string? Status { get; set; }
    }
}
