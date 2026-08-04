using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class RawMaterial
    {
        [Key]
        public int MaterialId { get; set; }

        [Required]
        public string MaterialCode { get; set; }

        [Required]
        public string MaterialName { get; set; }

        public string Category { get; set; }

        public string Unit { get; set; }

        public int CurrentStock { get; set; }

        public int MinimumStock { get; set; }

        public decimal PurchasePrice { get; set; }

        public string SupplierName { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public int IssueQuantity { get; set; }

        public string ApprovedBy { get; set; }

        public string ApprovalStatus { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }


    }
}
