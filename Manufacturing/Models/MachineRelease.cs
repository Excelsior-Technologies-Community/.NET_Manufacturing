using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class MachineRelease
    {
        [Key]
        public int ReleaseId { get; set; }

        [Required]
        public int MachineId { get; set; }

        [Required]
        public int ProductionOrderId { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        public string ReleasedBy { get; set; }

        public decimal RunningHours { get; set; }

        public string Status { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public string MachineName { get; set; }

        public string OrderNo { get; set; } 
    }
}
