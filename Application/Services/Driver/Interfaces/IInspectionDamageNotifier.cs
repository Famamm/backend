using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public interface IInspectionDamageNotifier
    {
        Task NotifyNewDamageAsync(Inspection inspection, IReadOnlyCollection<int> previousNotOkItemIds);
    }
}
