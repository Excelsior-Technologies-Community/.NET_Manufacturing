using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class StockAdjustment
    {

        [Key]
        public int AdjustmentId { get; set; }

        [Required]
        public int MaterialId { get; set; }

        [Required]
        public int CurrentStock { get; set; }

        [Required]
        public int AdjustmentQuantity { get; set; }

        public string AdjustmentType { get; set; }

        [Required]
        public DateTime AdjustmentDate { get; set; }

        public string AdjustedBy { get; set; }

        public string Reason { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public string MaterialName { get; set; }
    }
}
