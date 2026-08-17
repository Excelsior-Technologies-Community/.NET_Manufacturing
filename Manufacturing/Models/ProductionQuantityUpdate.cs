using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class ProductionQuantityUpdate
    {
        [Key]
        public int QuantityUpdateId { get; set; }

        [Required]
        public int ProductionOrderId { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public DateTime UpdateDate { get; set; }

        [Required]
        public int PlannedQuantity { get; set; }

        [Required]
        public int ProducedQuantity { get; set; }

        public int RejectedQuantity { get; set; }

        [Required]
        public int RemainingQuantity { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? OrderNo { get; set; }

        public string? MachineName { get; set; }

        public string? EmployeeName { get; set; }
    }
}
