using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public record NotificationContent(NotificationType Type, string Title, string Message, int ReferenceId);
}
