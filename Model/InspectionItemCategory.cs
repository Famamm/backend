namespace fleetops_backend.Models
{
    public class InspectionItemCategory
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        public ICollection<InspectionItem> Items { get; set; } = new List<InspectionItem>();
    }
}