using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class OperatorAssignment
    {
        [Key]
        public int OperatorAssignmentId { get; set; }

        [Required]
        public int ProductionOrderId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int ShiftId { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; }

        public string? Status { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        // Display Purpose
        public string? OrderNo { get; set; }

        public string? EmployeeName { get; set; }

        public string? ShiftName { get; set; }
    }
}
