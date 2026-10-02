using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public required string Token { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public required DateTimeOffset ExpiresAt { get; set; }

        public DateTimeOffset? RevokedAt { get; set; }

        public bool IsRevoked => RevokedAt.HasValue;

        public bool IsValid => !IsRevoked && DateTimeOffset.UtcNow < ExpiresAt;
    }
}
