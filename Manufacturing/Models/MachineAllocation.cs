using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class MachineAllocation
    {
        [Key]
        public int AllocationId { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        public int ProductionOrderId { get; set; }

        [Required]
        public DateTime AllocationDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public string AllocatedBy { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public string MachineName { get; set; }

        public string OrderNo { get; set; }
    }
}
