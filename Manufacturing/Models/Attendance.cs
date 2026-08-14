using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Manufacturing.Models
{
    public class Attendance
    {
        [Key]
        public int AttendanceId { get; set; }

        [Required(ErrorMessage = "Please select an employee.")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Please select attendance date.")]
        public DateTime AttendanceDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please select attendance status.")]
        public string? Status { get; set; } = "Present";

        public TimeSpan? CheckInTime { get; set; }

        public TimeSpan? CheckOutTime { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ValidateNever]
        public string? EmployeeName { get; set; }
    }
}