using System.ComponentModel.DataAnnotations.Schema;

namespace fleetops_backend.Models
{
    public class InspectionPhoto
    {
        public int Id { get; set; }

        [Column("inspection_id")]
        public int InspectionId { get; set; }
        public Inspection? Inspection { get; set; }

        public bool IsRequired { get; set; }
        public string ImageUrlPath { get; set; } = string.Empty;

        public ICollection<InspectionResult> Result { get; set; } = new List<InspectionResult>();
    }
}