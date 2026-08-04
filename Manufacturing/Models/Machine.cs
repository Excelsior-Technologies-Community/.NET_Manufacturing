using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Manufacturing.Models
{
    public class Machine
    {
        [Key]
        public int MachineId { get; set; }

        [Required]
        public string MachineCode { get; set; }

        [Required]
        public string MachineName { get; set; }

        public string MachineType { get; set; }

        public string Manufacturer { get; set; }

        public DateTime? PurchaseDate { get; set; }

        public DateTime? LastMaintenanceDate { get; set; }

        public DateTime? NextMaintenanceDate {  get; set; }

        public string Status {  get; set; }
        public string AllocationStatus { get; set; }

        public string AllocatedTo {  get; set; }

        public string ApprovedBy {  get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
 