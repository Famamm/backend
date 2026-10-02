using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public class InspectionItem
    {
        public int Id { get; set; }

        [Column("inspection_item_category_id")]
        public int InspectionItemCategoryId { get; set; }
        public InspectionItemCategory? Category { get; set; }

        public required string Name { get; set; }
        public bool IsRequired { get; set; }

        public ICollection<InspectionResult> Result { get; set; } = new List<InspectionResult>();
    }
}