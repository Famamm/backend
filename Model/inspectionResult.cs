using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public enum InspectionCondition { NotOk = 0, Ok = 1 }

    public class InspectionResult
    {
        public int Id { get; set; }

        [Column("inspection_id")]
        public int InspectionId { get; set; }
        public Inspection? Inspection { get; set; }

        [Column("item_id")]
        public int ItemId { get; set; }
        public InspectionItem? Item { get; set; }

        [Column("photo_id")]
        public int? PhotoId { get; set; }
        public InspectionPhoto? Photo { get; set; }

        public InspectionCondition Condition { get; set; }
        public string? Note { get; set; }
    }
}