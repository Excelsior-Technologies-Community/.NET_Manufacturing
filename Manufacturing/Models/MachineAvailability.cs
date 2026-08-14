using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class MachineAvailability
    {
        [Key]
        public int AvailabilityId { get; set; }

        [Required]
        public int MachineId { get; set; }

        public string AvailabilityStatus { get; set; }

        [Required]
        public DateTime AvailableFrom { get; set; }

        public DateTime? AvailableTo { get; set; }

        public string CurrentStatus { get; set; }

        public string Remarks { get; set; }

        public DateTime CreatedDate { get; set; }
        public string MachineName { get; set; }
    }
}
