using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class ProductionWorkStatus
    {
        [Key]
        public int WorkStatusId { get; set; }

        [Required]
        public int ProductionOrderId { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        public int EmployeeId { get; set; }

        public DateTime? StartDate { get; set; }

        public TimeSpan? StartTime { get; set; }

        public DateTime? PauseDate { get; set; }

        public TimeSpan? PauseTime { get; set; }

        public DateTime? ResumeDate { get; set; }

        public TimeSpan? ResumeTime { get; set; }

        public DateTime? CompletionDate { get; set; }

        public TimeSpan? CompletionTime { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }
        public string OrderNo { get; set; }

        public string MachineName { get; set; }

        public string EmployeeName { get; set; }
    }
}
