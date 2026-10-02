using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        public NotificationType Type { get; set; }

        public required string Title { get; set; }

        public required string Message { get; set; }

        public int ReferenceId { get; set; }

        public bool IsRead { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
