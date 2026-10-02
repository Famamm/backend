using fleetops_backend.Models;

namespace fleetops_backend.Application.Services
{
    public static class InspectionDamageDetector
    {
        public static List<int> GetNotOkItemIds(IEnumerable<InspectionResult> results)
        {
            return results
                .Where(r => r.Condition == InspectionCondition.NotOk)
                .Select(r => r.ItemId)
                .Distinct()
                .ToList();
        }

        public static List<int> FindNewNotOkItemIds(
            IEnumerable<int> previousNotOkItemIds,
            IEnumerable<InspectionResult> currentResults)
        {
            return GetNotOkItemIds(currentResults)
                .Except(previousNotOkItemIds)
                .ToList();
        }
    }
}
