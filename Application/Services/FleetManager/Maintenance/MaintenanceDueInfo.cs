namespace fleetops_backend.Application.Services
{
    public enum MaintenanceDueState
    {
        Upcoming,
        DueSoon,
        Overdue
    }

    public record MaintenanceDueInfo(MaintenanceDueState State, int? DaysRemaining, int? KmRemaining);
}
