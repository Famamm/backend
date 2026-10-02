namespace fleetops_backend.Application.DTOs
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public required string Type { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
        public int ReferenceId { get; set; }
        public bool IsRead { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
