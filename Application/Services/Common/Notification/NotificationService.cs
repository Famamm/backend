using fleetops_backend.Infrastructure.Data;
using fleetops_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace fleetops_backend.Application.Services
{
    public class NotificationService : INotificationService
    {
        private const string FleetManagerRole = "FleetManager";
        private const string AdminRole = "Admin";

        private readonly AppDbContext _context;

        public NotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task NotifyFleetManagersAsync(
            int organizationId,
            NotificationContent content,
            IReadOnlyCollection<int>? additionalUserIds = null)
        {
            var fleetManagerIds = await _context.Users
                .Where(u => u.OrganizationId == organizationId && u.Role!.Name == FleetManagerRole)
                .Select(u => u.Id)
                .ToListAsync();

            await NotifyUsersAsync(fleetManagerIds.Concat(additionalUserIds ?? []).ToList(), content);
        }

        public async Task NotifyAdminsAsync(
            int organizationId,
            NotificationContent content,
            IReadOnlyCollection<int>? additionalUserIds = null)
        {
            var adminIds = await _context.Users
                .Where(u => u.OrganizationId == organizationId && u.Role!.Name == AdminRole)
                .Select(u => u.Id)
                .ToListAsync();

            await NotifyUsersAsync(adminIds.Concat(additionalUserIds ?? []).ToList(), content);
        }

        public async Task NotifyAdminsAndFleetManagersAsync(
            int organizationId,
            NotificationContent content,
            IReadOnlyCollection<int>? additionalUserIds = null)
        {
            var recipientIds = await _context.Users
                .Where(u => u.OrganizationId == organizationId
                    && (u.Role!.Name == AdminRole || u.Role!.Name == FleetManagerRole))
                .Select(u => u.Id)
                .ToListAsync();

            await NotifyUsersAsync(recipientIds.Concat(additionalUserIds ?? []).ToList(), content);
        }

        public async Task NotifyUsersAsync(IReadOnlyCollection<int> userIds, NotificationContent content)
        {
            var recipientIds = userIds.Distinct().ToList();

            if (recipientIds.Count == 0)
            {
                return;
            }

            _context.Notifications.AddRange(recipientIds.Select(userId => new Notification
            {
                UserId = userId,
                Type = content.Type,
                Title = content.Title,
                Message = content.Message,
                ReferenceId = content.ReferenceId
            }));

            await _context.SaveChangesAsync();
        }
    }
}
