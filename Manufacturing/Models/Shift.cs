using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class Shift
    {
        [Key]
        public int ShiftId { get; set; }

        [Required]
        public string ShiftCode { get; set; }

        [Required]
        public string ShiftName { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public decimal TotalHours { get; set; }

        public string SupervisorName { get; set; }

        public string Status { get; set; }

        public string Description { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
