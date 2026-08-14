using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class MaintenanceRequest
    {
        [Key]
        public int RequestId { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        public DateTime RequestDate { get; set; }

        [Required]
        public string ProblemDescription { get; set; }

        public string Priority { get; set; }

        public string RequestedBy { get; set; }

        public string AssignedTo { get; set; }

        public string Status { get; set; }

        public DateTime? CompletionDate { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        // Display Property
        public string MachineName { get; set; }
    }
}
