using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class ProductionOrder
    {
        [Key]
        public int ProductionOrderId { get; set; }

        [Required]
        public string OrderNo { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string Unit { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Priority { get; set; }

        public string Status { get; set; }

        public string ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string Remarks { get; set; }

        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}
