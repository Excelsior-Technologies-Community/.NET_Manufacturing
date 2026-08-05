using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required]
        public string EmployeeCode { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Gender { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public int DepartmentId { get; set; }

        public int ShiftId { get; set; }

        public string Designation { get; set; }

        public DateTime? JoinDate { get; set; }

        public decimal Salary { get; set; }

        public string AttendanceStatus { get; set; }

        public string Status { get; set; }

        public string Address { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? DepartmentName { get; set; }

        public string? ShiftName { get; set; }
    }
}
