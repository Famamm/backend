namespace fleetops_backend.Application.DTOs
{
    public class UpdateDamageReportStatusDTO
    {
        // Reported, InReview, InRepair, Resolved, Rejected
        public required string Status { get; set; }

        public string? ReviewNote { get; set; }
    }
}
