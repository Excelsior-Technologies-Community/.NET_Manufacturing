using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class ProductionSchedules
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        public int ProductionOrderId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string Priority { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public string OrderNo { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }
    }
}
