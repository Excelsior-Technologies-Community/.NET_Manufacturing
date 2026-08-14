using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Manufacturing.Models
{
    public class LeaveRequest
    {
        [Key]
        public int LeaveRequestId { get; set; }

        [Required(ErrorMessage = "Please select an employee.")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Please select leave type.")]
        public string? LeaveType { get; set; }

        [Required(ErrorMessage = "Please select start date.")]
        public DateTime FromDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Please select end date.")]
        public DateTime ToDate { get; set; } = DateTime.Today;

        public int TotalDays { get; set; }

        public string? Reason { get; set; }

        public DateTime RequestedDate { get; set; } = DateTime.Today;

        public string? ApprovedBy { get; set; }

        public DateTime? ApprovalDate { get; set; }

        public string? Status { get; set; } = "Pending";

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ValidateNever]
        public string? EmployeeName { get; set; }
    }
}
