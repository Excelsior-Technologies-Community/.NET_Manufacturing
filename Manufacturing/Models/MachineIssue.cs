using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class MachineIssue
    {
        [Key]
        public int IssueId { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        public int ProductionOrderId { get; set; }

        public string? ReportedBy { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        public string? IssueType { get; set; }

        [Required]
        public string ProblemDescription { get; set; }

        public string? Priority { get; set; }

        public string? Status { get; set; }

        public DateTime? ResolvedDate { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? MachineName { get; set; }

        public string? OrderNo { get; set; }
    }
}
