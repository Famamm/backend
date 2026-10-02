using System.ComponentModel.DataAnnotations.Schema;
namespace fleetops_backend.Models;

public class User
{
    public int Id { get; set; }
    public required string FullName { get; set; }
    public required string Username { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? ProfilePhoto { get; set; }
    public required string PasswordHash { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }
    public  Role? Role { get; set; }

    [Column("organization_id")]
    public int OrganizationId { get; set; }

    [ForeignKey(nameof(OrganizationId))]
    public Organization? Organization { get; set; }

    public Driver? Driver { get; set; }

    public FleetManager? FleetManager { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}