using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class MaterialIssue
    {
        [Key]
        public int IssueId { get; set; }

        [Required]
        public int MaterialId { get; set; }

        [Required]
        public int ProductionOrderId { get; set; }

        [Required]
        public int IssueQuantity { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        public string IssuedBy { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }
            public string MaterialName { get; set; }

        public string OrderNo { get; set; }
    }
}
